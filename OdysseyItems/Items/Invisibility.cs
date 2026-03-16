using DSMOOFramework.Config;
using DSMOOServer.API.Events;
using DSMOOServer.API.Events.Args;

namespace OdysseyItems.Items;

[Item(Name = "Invisibility", ShortName = "Invis", BodyName = "MarioTailCoat", CapName = "MarioTailCoat")]
public class Invisibility(EventManager eventManager, ConfigHolder<Config> holder) : Item
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

        args.Invisible = true;
    }

    private async Task ReturnItemTask(CancellationToken ct)
    {
        await Task.Delay(holder.Config.InvisibilityTime * 1000, ct);
        if (ct.IsCancellationRequested)
            return;
        ReturnItemToPool();
    }
}