using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using Respawning;
using System;
using System.Linq;

using Random = UnityEngine.Random;

namespace CustomUnit.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Spawn : ICommand, IUsageProvider
    {
        public string Command => "spawn";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "Spawns a custom unit";

        public string[] Usage => new[]
        {
            "Team",
            "Override condition"
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

            SpawnableTeamType team;
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

                        if (!bool.TryParse(arguments.At(1).ToLower(), out var cond))
                        {
                            response = new ArgumentOutOfRangeException("Override conditions").Message;
                            return false;
                        }

                        var num = Random.Range(0, 7);

                        if (cond)
                        {
                            EventHadlers.Spawn(unit, spawn.GetRange(num, RespawnManager.SpawnableTeams[unit.Team].MaxWaveSize - num), unit.Team);

                            response = "Successfully spawned!";
                            return true;
                        }
                        else
                        {
                            if (unit.UseChance)
                            {
                                if (Random.Range(0, 101) > 65)
                                {
                                    EventHadlers.Spawn(unit, spawn.GetRange(num, RespawnManager.SpawnableTeams[unit.Team].MaxWaveSize - num), unit.Team);

                                    response = "Successfully spawned!";
                                    return true;
                                }
                                else
                                {
                                    response = "Your team has not enough chance to spawn. You can override this checks by setting \"Override Conditions\" command param to 'true'";
                                    return false;
                                }
                            }
                            else
                            {
                                switch (unit.Team)
                                {
                                    case SpawnableTeamType.NineTailedFox when Respawn.NtfTickets < Plugin.Tickets[unit]:
                                        EventHadlers.Spawn(unit, spawn.GetRange(num, RespawnManager.SpawnableTeams[unit.Team].MaxWaveSize - num), unit.Team);

                                        response = "Successfully spawned!";
                                        return true;
                                    case SpawnableTeamType.ChaosInsurgency when Respawn.ChaosTickets < Plugin.Tickets[unit]:
                                        EventHadlers.Spawn(unit, spawn.GetRange(num, RespawnManager.SpawnableTeams[unit.Team].MaxWaveSize - num), unit.Team);

                                        response = "Successfully spawned!";
                                        return true;
                                    default:
                                        response = "Your team has not enough tickets to spawn. You can override this checks by setting \"Override Conditions\" command param to 'true'";
                                        return false;
                                }
                            }
                        }
                    }
                    else
                    {
                        response = $"Cannot find unit with given name ({arguments.At(0)})";
                        return false;
                    }
            }

            RespawnManager.Singleton.ForceSpawnTeam(team);

            response = "Spawned in-game team";
            return true;
        }
    }
}
