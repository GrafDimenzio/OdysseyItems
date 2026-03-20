using DSMOOFramework.Commands;
using DSMOOFramework.Config;
using DSMOOServer.API.Serialized;
using DSMOOServer.Logic;

namespace OdysseyItems;

[Command(
    CommandName = "itemspawn",
    Aliases = [],
    Description = "Adds the current Position of the first player on the server to the Config",
    Parameters = []
)]
public class SpawnCommand(PlayerManager manager, ConfigHolder<ItemLocations> holder) : Command
{
    public override CommandResult Execute(string command, string[] args, ICommandSender sender)
    {
        if (manager.PlayerCount == 0)
            return new CommandResult
            {
                Message = "No Player is connected",
                ResultType = ResultType.Error
            };

        var player = manager.Players[0];
        if (!holder.Config.Locations.TryGetValue(player.Stage, out var list))
        {
            list = new List<SerializedVector3>();
            holder.Config.Locations[player.Stage] = list;
        }

        list.Add(player.Position);
        holder.SaveConfig();
        return $"Added Position of {player.Name} to the Config";
    }
}