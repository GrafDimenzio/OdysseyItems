using System.Collections.ObjectModel;
using System.Numerics;
using DSMOOFramework.Analyzer;
using DSMOOFramework.Config;
using DSMOOFramework.Controller;
using DSMOOFramework.Events;
using DSMOOFramework.Logger;
using DSMOOFramework.Plugins;
using DSMOOServer;
using DSMOOServer.API.Events;
using DSMOOServer.API.Events.Args;
using DSMOOServer.API.GameModes;
using DSMOOServer.API.Player;
using DSMOOServer.API.Serialized;
using DSMOOServer.Logic;
using DSMOOServer.Network.Packets;
using OdysseyItems.Items;

namespace OdysseyItems;

[Plugin(
    Name = "Odyssey Items",
    Author = "Dimenzio",
    Description = "Adds Items to the game modes",
    Version = "1.0.0"
)]
public class ItemManager(
    EventManager eventManager,
    PlayerManager playerManager,
    DummyManager dummyManager,
    GameModeManager gameModeManager,
    Analyzer analyzer,
    ObjectController objectController,
    ConfigHolder<ServerMainConfig> mainHolder,
    ILogger eventLogger,
    ConfigHolder<ItemLocations> locationsHolder) : Plugin<Config>
{
    private readonly List<Item> _activeItems = [];

    private readonly Dictionary<ItemAttribute, Type> _items = [];
    private readonly Random _random = new();

    public readonly EventReactor<ItemEventArgs> OnItemCollected = new(eventLogger);
    public readonly EventReactor<ItemEventArgs> OnItemReturned = new(eventLogger);
    private CancellationTokenSource? _source;

    public ReadOnlyDictionary<ItemAttribute, Type> Items => _items.AsReadOnly();
    public List<Item> ActiveItems => _activeItems.ToList();

    public override void Initialize()
    {
        //TODO: Once Nuget is updated subscribe to player disconnect and take away item
        eventManager.OnPlayerState.Subscribe(OnPlayerState);
        eventManager.OnPreGameStart.Subscribe(OnPreGameStart);
        eventManager.OnGameEnd.Subscribe(OnGameEnd);
        analyzer.OnAnalyze.Subscribe(OnAnalyze);
    }

    public void RedistributeItems()
    {
        if (gameModeManager.ActiveGame == null || _activeItems.Count == 0)
            return;

        foreach (var item in _activeItems)
            if (item.ReadyToCollect)
                item.ReturnItemToPool();

        var possibleStages = new List<string>();
        if (gameModeManager.ActiveGame.StagePreset.AllowAll)
        {
            possibleStages.AddRange(locationsHolder.Config.Locations.Keys);
        }
        else
        {
            var allowedStages = gameModeManager.ActiveGame.StagePreset.StartingStages.ToList();
            allowedStages.AddRange(gameModeManager.ActiveGame.StagePreset.AllowedStages);
            foreach (var stage in allowedStages)
                if (locationsHolder.Config.Locations.ContainsKey(stage))
                    possibleStages.Add(stage);
        }

        var usedPositions = new Dictionary<string, List<SerializedVector3>>();

        foreach (var item in _activeItems.OrderBy(_ => _random.Next()))
        {
            if (item.ActivePlayer != null)
                continue;

            var counter = 0;
            while (counter < 10)
            {
                var stage = possibleStages[_random.Next(possibleStages.Count)];
                var locations = locationsHolder.Config.Locations[stage];
                var position = locations[_random.Next(locations.Count)];

                if (!usedPositions.ContainsKey(stage))
                    usedPositions[stage] = [];
                var isValid = true;
                foreach (var usedPos in usedPositions[stage])
                {
                    if (usedPos != position) continue;
                    counter++;
                    isValid = false;
                    break;
                }

                if (!isValid)
                    continue;

                usedPositions[stage].Add(position);

                item.Dummy.Stage = stage;
                item.Dummy.Position = position;
                item.Dummy.IsIt = false;
                item.ReadyToCollect = true;
                item.Dummy.Act = 419;
                counter = 10;
            }
        }
    }

    private List<ItemAttribute> PickRandomItems()
    {
        var items = new List<ItemAttribute>();
        var possibleItems = _items.Keys.Where(x => Config.EnabledItems.Contains(x.Name)).ToList();
        if (possibleItems.Count == 0)
        {
            Logger.Warn("No Items could be found that can be spawned. Check your EnabledItems Config!");
            return [];
        }
        for (var i = 0; i < Config.ItemAmount; i++) items.Add(possibleItems[_random.Next(possibleItems.Count)]);
        return items;
    }

    private Item? CreateItem(ItemAttribute attribute)
    {
        var item = (Item?)objectController.CreateObject(_items[attribute]);
        if (item == null)
            return null;
        item.Info = attribute;
        item.Dummy = dummyManager.CreateDummy(Config.ShowItemNameOnPlayerList ? $"{attribute.ShortName} Item" : "Item")
            .GetAwaiter().GetResult();
        item.Costume = new CostumePacket { BodyName = attribute.BodyName, CapName = attribute.CapName };
        item.Dummy.Costume = item.Costume;
        item.Dummy.CurrentGameMode = GameMode.HideAndSeek;
        item.Dummy.IsIt = true;
        _activeItems.Add(item);
        return item;
    }

    private void OnAnalyze(AnalyzeEventArgs args)
    {
        if (!args.Is<Item>()) return;
        var attribute = args.GetAttribute<ItemAttribute>();
        if (attribute == null) return;
        _items[attribute] = args.Type;
    }

    private void OnPreGameStart(GameEventArgs args)
    {
        if (mainHolder.Config.MaxPlayers < Config.ItemAmount + playerManager.Players.Count)
        {
            Logger.Warn("Max Players amount are too low! OdysseyItems will not be enabled");
            return;
        }

        var itemsAttributes = PickRandomItems();
        if (itemsAttributes.Count == 0)
            return;
        foreach (var itemsAttribute in itemsAttributes) CreateItem(itemsAttribute);

        _source?.Cancel(); // Just a fail safe
        _source = new CancellationTokenSource();
        Task.Run(() => RedistributeRun(_source.Token));
    }

    private void OnGameEnd(GameEventArgs args)
    {
        foreach (var item in _activeItems.ToList()) item.Dispose();
        _activeItems.Clear();
        _source?.Cancel();
    }

    private void OnPlayerState(PlayerStateEventArgs args)
    {
        foreach (var item in _activeItems)
        {
            if (!item.ReadyToCollect || args.Player.Stage != item.Dummy.Stage || args.Player.IsIt)
                continue;

            var distanceSquared = Vector3.DistanceSquared(item.Dummy.Position, args.Player.Position);
            if (distanceSquared <= 20000)
            {
                //Race Condition
                if (!item.ReadyToCollect)
                    continue;

                item.ReadyToCollect = false;
                item.Dummy.IsIt = true;
                item.Dummy.Position = new Vector3(0, -10000f, 0);
                item.Dummy.Stage = "";
                item.ActivePlayer = args.Player;
                OnItemCollected.RaiseEvent(new ItemEventArgs { Item = item, Player = args.Player });
                try
                {
                    item.OnCollected();
                }
                catch (Exception e)
                {
                    Logger.Error("Error in Item OnCollected method", e);
                    item.ReturnItemToPool();
                }
            }
        }
    }

    private async Task RedistributeRun(CancellationToken token)
    {
        await Task.Delay(Config.InitialWaitTimeToSpawn * 1000, token);

        while (!token.IsCancellationRequested)
        {
            RedistributeItems();
            await Task.Delay(Config.RespawnTime * 1000, token);
        }
    }
}