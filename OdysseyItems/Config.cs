using DSMOOFramework.Config;
using DSMOOServer.API.Serialized;

namespace OdysseyItems;

[Config(Name = "odyssey_items")]
public class Config : IConfig
{
    public string[] EnabledItems { get; set; } = ["Delay", "Escape", "Invisibility", "Star", "Offset"];
    public bool SeekerDestroyItem { get; set; } = true;
    public int ItemAmount { get; set; } = 5;
    public bool ShowItemNameOnPlayerList { get; set; } = true;
    public int InitialWaitTimeToSpawn { get; set; } = 30;
    public int RespawnTime { get; set; } = 90;
    public int InvisibilityTime { get; set; } = 15;
    public int InvincibilityTime { get; set; } = 25;
    public bool AlwaysOtherStageOnEscape { get; set; } = false;
    public int OffsetTime { get; set; } = 30;
    public SerializedVector3 Offset { get; set; } = new(0, 500, 0);
    public int DelayTime { get; set; } = 30;
    public int AmountOfDelay { get; set; } = 1;
}