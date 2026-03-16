using System.Numerics;
using DSMOOFramework.Config;
using DSMOOServer.API.GameModes;
using DSMOOServer.API.Stage;

namespace OdysseyItems.Items;

[Item(Name = "Escape", ShortName = "Escape", BodyName = "MarioSpaceSuit", CapName = "MarioSpaceSuit")]
public class Escape(GameModeManager manager, StageManager stageManager, ConfigHolder<Config> holder) : Item
{
    private readonly Random _random = new();

    public override void OnCollected()
    {
        //Should never happen but my ide yells at me
        if (ActivePlayer == null || manager.ActiveGame == null)
        {
            ReturnItemToPool();
            return;
        }

        List<string> possibleStages;
        
        if (manager.ActiveGame.StagePreset.AllowAll)
        {
            possibleStages = stageManager.Stages.Select(x => x.StageName).ToList();
        }
        else
        {
            possibleStages = manager.ActiveGame.StagePreset.AllowedStages.ToList();
            possibleStages.AddRange(manager.ActiveGame.StagePreset.StartingStages);
        }
        
        if (holder.Config.AlwaysOtherStageOnEscape)
            foreach (var stageName in possibleStages.ToList())
                if (stageName == ActivePlayer.Stage)
                    possibleStages.Remove(stageName);
        
        if (possibleStages.Count == 0)
            possibleStages = [manager.ActiveGame.StagePreset.StartingStages[0]];
        if (possibleStages.Count == 0)
        {
            ReturnItemToPool();
            return;
        }
        
        var stage = possibleStages[_random.Next(0, possibleStages.Count)];
        var warps = stageManager.GetStageInfo(stage)?.Warps.ToList() ?? [];
        foreach (var warpName in warps.ToList())
        {
            if (warpName.Position == Vector3.Zero)
                warps.Remove(warpName);
        }

        var warp = "";
        if (warps.Count > 0)
        {
            warp = warps[_random.Next(warps.Count)].Name;
        }

        ActivePlayer.ChangeStage(stage, warp);
        ReturnItemToPool();
    }
}