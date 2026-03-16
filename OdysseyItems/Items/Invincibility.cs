using DSMOOFramework.Config;
using DSMOOServer.API.Events;
using DSMOOServer.API.Events.Args;
using DSMOOServer.Logic;
using DSMOOServer.Network.Packets;

namespace OdysseyItems.Items;

[Item(Name = "Star", ShortName = "Star", BodyName = "MarioClown", CapName = "MarioClown")]
public class Invincibility(EventManager eventManager, PlayerManager playerManager, ConfigHolder<Config> holder) : Item
{
    private CancellationTokenSource _source;

    public override void AfterInject()
    {
        eventManager.OnPacketReceived.Subscribe(OnPacket);
    }

    public override void Dispose()
    {
        eventManager.OnPacketReceived.Unsubscribe(OnPacket);
        base.Dispose();
    }

    public override void OnCollected()
    {
        foreach (var player in playerManager.Players)
        {
            if (!player.IsIt || player == ActivePlayer) continue;
            //this will desync the player so that everyone is a hider and no one can tag him that way
            ActivePlayer?.Send(new TagPacket()
            {
                IsIt = false,
                GameMode = player.CurrentGameMode,
                UpdateType = TagPacket.TagUpdate.State
            }, player.Id);
        }
        _source?.Cancel();
        _source = new CancellationTokenSource();
        Task.Run(() => ReturnItemTask(_source.Token), _source.Token);
    }

    public override void OnReturn()
    {
        foreach (var player in playerManager.Players)
        {
            if (player == ActivePlayer) continue;
            //This will sync the player to the normal state
            ActivePlayer?.Send(new TagPacket
            {
                IsIt = player.IsIt,
                GameMode = player.CurrentGameMode,
                UpdateType = TagPacket.TagUpdate.State
            }, player.Id);
        }
        _source?.Cancel();
    }

    private void OnPacket(PacketReceivedEventArgs args)
    {
        if (ActivePlayer == null || args.Sender.Id == ActivePlayer.Id)
            return;
        
        switch (args.Packet)
        {
            case TagPacket tagPacket:
                if (!tagPacket.UpdateType.HasFlag(TagPacket.TagUpdate.State) || !tagPacket.IsIt)
                    return;
                //Make sure that no one can bypass the item by switching the game mode quickly
                args.SpecificReplacePackets[ActivePlayer.Id] = tagPacket with { IsIt = false };
                break;
        }
    }

    private async Task ReturnItemTask(CancellationToken ct)
    {
        await Task.Delay(holder.Config.InvincibilityTime * 1000, ct);
        if (ct.IsCancellationRequested)
            return;
        ReturnItemToPool();
    }
}