using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using Respawning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomUnit.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Spawn : ICommand, IUsageProvider
    {
        public string Command => "spawn";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "Spawns a custom unit";

        public string[] Usage => new string[]
        {
            "Team"
        };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("cu.spawn"))
            {
                response = "You don't have permissions to execute this command";
                return false;
            }
            if (arguments.Count == 0)
            {
                response = this.DisplayCommandUsage();
                return false;
            }

            var team = SpawnableTeamType.None;
            switch (arguments.At(0).ToLower())
            {
                case "ntf":
                    team = SpawnableTeamType.NineTailedFox;
                    break;
                case "ci":
                    team = SpawnableTeamType.ChaosInsurgency;
                    break;
                default:
                    if (Plugin.Configs.Values.ToList().Exists(x => x.UnitName.ToLower() == arguments.At(0).ToLower()))
                    {
                        var spawn = Player.List.Where(x => RespawnManager.Singleton.CheckSpawnable(x.ReferenceHub))
                            .ToList();
                        var unit = Plugin.Configs.Values.ToList()
                            .Find(x => x.UnitName.ToLower() == arguments.At(0).ToLower());
                        
                        spawn.ShuffleList();

                        EventHadlers.Spawn(unit, spawn.GetRange(6, RespawnManager.SpawnableTeams[unit.Team].MaxWaveSize + 6), unit.Team);

                        response = null;
                        return true;
                    }
                    else
                    {
                        response = null;
                        return false;
                    }
            }

            var spec = Player.List.Where(x => x.Role.Type == PlayerRoles.RoleTypeId.Spectator).ToList();
            var pl = spec.GetRange(6, 10);
            bool suc = EventHadlers.SpawnChance(team, pl);

            response = suc ? "Successfully spawned" : "Some errors occurred while spawning";
            return suc;
        }
    }
}
