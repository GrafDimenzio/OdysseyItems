using DSMOOFramework.Managers;
using DSMOOFramework.Plugins;
using DSMOOPlus;
using OdysseyItems;

namespace OdysseyItemsPlus;

[Plugin(
    Name = "Odyssey Items Plus",
    Description = "Adds SMOO+ features to Odyssey items",
    Author = "Dimenzio",
    Version = "1.0.0",
    Repository = "https://github.com/GrafDimenzio/OdysseyItems"
    )]
public class OdysseyItemsPlus(ItemManager itemManager) : Plugin<Config>
{
    public override void Initialize()
    {
        itemManager.OnItemCollected.Subscribe(OnItemCollect);
        itemManager.OnItemReturned.Subscribe(OnItemReturned);
        itemManager.OnItemDestroyed.Subscribe(OnItemDestroyed);
        Logger.Info("Odyssey Items Plus initialized");
    }

    private void OnItemDestroyed(ItemEventArgs eventArgs)
    {
        eventArgs.Player.GetComponent<PlayerPlus>()!.SendMessage($"You destroyed the {eventArgs.Item.Info.Name} Item");
    }

    private void OnItemCollect(ItemEventArgs eventArgs)
    {
        eventArgs.Player.GetComponent<PlayerPlus>()!.SendMessage($"You collected the {eventArgs.Item.Info.Name} Item");
    }

    private void OnItemReturned(ItemEventArgs eventArgs)
    {
        eventArgs.Player.GetComponent<PlayerPlus>()!.SendMessage(
            $"The effect of {eventArgs.Item.Info.Name} has run out");
    }
}