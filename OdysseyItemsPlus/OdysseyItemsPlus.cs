using DSMOOFramework.Managers;
using DSMOOPlus;
using OdysseyItems;

namespace OdysseyItemsPlus;

public class OdysseyItemsPlus(ItemManager itemManager) : Manager
{
    public override void Initialize()
    {
        itemManager.OnItemCollected.Subscribe(OnItemCollect);
        itemManager.OnItemReturned.Subscribe(OnItemReturned);
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