using System.Numerics;
using DSMOOFramework.Config;
using DSMOOServer.API.Events;
using DSMOOServer.API.Events.Args;
using DSMOOServer.Network.Packets;

namespace OdysseyItems.Items;

[Item(Name = "Offset", ShortName = "Offset", BodyName = "Mario64", CapName = "Mario64")]
public class Offset(EventManager eventManager, ConfigHolder<Config> holder) : Item
{
    private CancellationTokenSource _source;

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
    }

    private void OnPlayerState(PlayerStateEventArgs args)
    {
        if (args.Player != ActivePlayer)
            return;

        var pos = new Vector3(args.Player.Position.X + holder.Config.Offset.X,
            args.Player.Position.Y + holder.Config.Offset.Y, args.Player.Position.Z + holder.Config.Offset.Z);

        args.Packet = new PlayerPacket
        {
            Act = args.Packet.Act,
            SubAct = args.Packet.SubAct,
            AnimationBlendWeights = args.Packet.AnimationBlendWeights,
            Rotation = args.Packet.Rotation,
            Position = pos
        };
    }

    private async Task ReturnItemTask(CancellationToken ct)
    {
        await Task.Delay(holder.Config.OffsetTime * 1000, ct);
        if (ct.IsCancellationRequested)
            return;
        ReturnItemToPool();
    }
}