using DSMOOFramework.Config;

namespace OdysseyItemsPlus;

[Config(Name = "odyssey_items_plus")]
public class Config : IConfig
{
    public byte BonusHealth { get; set; } = 12;
}