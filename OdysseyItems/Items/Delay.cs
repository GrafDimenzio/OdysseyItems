using System.Collections.Concurrent;
using DSMOOFramework.Config;
using DSMOOServer.API.Events;
using DSMOOServer.API.Events.Args;
using DSMOOServer.Network.Packets;

namespace OdysseyItems.Items;

[Item(Name = "Delay", ShortName = "Delay", BodyName = "MarioShopman", CapName = "MarioShopman")]
public class Delay(EventManager eventManager, ConfigHolder<Config> holder) : Item
{
    private readonly ConcurrentQueue<PlayerPacket> _queue = new();
    private CancellationTokenSource _source = new();

    public override void AfterInject()
    {
        eventManager.OnPlayerState.Subscribe(OnPlayerState);
    }

    public override void Dispose()
    {
        eventManager.OnPlayerState.Unsubscribe(OnPlayerState);
        base.Dispose();
    }

    public override void OnCollected()
    {
        _source?.Cancel();
        _source = new CancellationTokenSource();
        Task.Run(() => ReturnItemTask(_source.Token), _source.Token);
    }

    public override void OnReturn()
    {
        _source?.Cancel();
        _queue.Clear();
    }

    private void OnPlayerState(PlayerStateEventArgs args)
    {
        if (args.Player != ActivePlayer)
            return;

        _queue.Enqueue(args.Packet);
        Task.Run(() => DelayPacket(_source.Token), _source.Token);

        if (_queue.TryPeek(out var packet)) args.Packet = packet;
    }

    private async Task DelayPacket(CancellationToken ct)
    {
        await Task.Delay(holder.Config.AmountOfDelay * 1000, ct);
        if (ct.IsCancellationRequested || ActivePlayer == null)
            return;
        _queue.TryDequeue(out _);
    }

    private async Task ReturnItemTask(CancellationToken ct)
    {
        await Task.Delay(holder.Config.DelayTime * 1000, ct);
        if (ct.IsCancellationRequested)
            return;
        ReturnItemToPool();
    }
}