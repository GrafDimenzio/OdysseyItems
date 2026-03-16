using System.Numerics;
using DSMOOFramework.Controller;
using DSMOOServer.API.Player;
using DSMOOServer.Network.Packets;

namespace OdysseyItems.Items;

public abstract class Item : IDisposable, IInject
{
    [Inject] public ItemManager ItemManager { get; set; }

    public Dummy Dummy { get; internal set; }

    public ItemAttribute Info { get; internal set; }

    public CostumePacket Costume { get; internal set; }

    public IPlayer? ActivePlayer { get; internal set; }

    public bool ReadyToCollect { get; set; }

    public virtual void Dispose()
    {
        ActivePlayer = null;
        Dummy.Dispose();
    }

    public virtual void AfterInject()
    {
    }

    public virtual void ReturnItemToPool()
    {
        if (ActivePlayer != null)
            ItemManager.OnItemReturned.RaiseEvent(new ItemEventArgs { Item = this, Player = ActivePlayer });
        OnReturn();
        ReadyToCollect = false;
        Dummy.Costume = Costume;
        Dummy.CurrentGameMode = GameMode.HideAndSeek;
        Dummy.IsIt = true;
        Dummy.Position = new Vector3(0, -10000f, 0);
        Dummy.Stage = "";
        ActivePlayer = null;
    }

    public virtual void OnCollected()
    {
    }

    public virtual void OnReturn()
    {
    }
}