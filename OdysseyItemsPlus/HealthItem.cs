using DSMOOFramework.Config;
using DSMOOPlus;
using OdysseyItems.Items;

namespace OdysseyItemsPlus;

[Item(Name = "Bonus Health", ShortName = "Health", BodyName = "MarioDoctor", CapName = "MarioDoctor")]
public class HealthItem(ConfigHolder<Config> holder) : Item
{
    public override void OnCollected()
    {
        ActivePlayer?.GetComponent<PlayerPlus>()?.SetHealth(holder.Config.BonusHealth);
        ReturnItemToPool();
    }
}