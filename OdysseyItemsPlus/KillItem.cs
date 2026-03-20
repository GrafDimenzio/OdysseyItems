using DSMOOFramework.Config;
using DSMOOPlus;
using DSMOOServer.API.GameModes;
using OdysseyItems.Items;

namespace OdysseyItemsPlus;

[Item(Name = "Kill", ShortName = "Kill", BodyName = "MarioBone", CapName = "MarioHappi")]
public class KillItem(ConfigHolder<Config> holder, GameModeManager gameModeManager) : Item
{
    public override void OnCollected()
    {
        if (ActivePlayer != null)
            foreach (var player in gameModeManager.ActiveGame?.Players ?? [])
            {
                if (player == ActivePlayer || !player.IsIt)
                    continue;

                player.GetComponent<PlayerPlus>()?.SendMessage($"You were killed by {ActivePlayer.Name}");
                player.GetComponent<PlayerPlus>()?.SetHealth(0);
            }

        ReturnItemToPool();
    }
}