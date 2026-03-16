using DSMOOFramework.Events;
using DSMOOServer.API.Player;
using OdysseyItems.Items;

namespace OdysseyItems;

public class ItemEventArgs : IEventArg
{
    public IPlayer Player { get; init; }
    public Item Item { get; init; }
}