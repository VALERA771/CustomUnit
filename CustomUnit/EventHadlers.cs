using CustomUnit.Configs;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using Respawning;
using System.Collections.Generic;
using System.Linq;
using CustomUnit.Additions;
using Random = System.Random;

namespace CustomUnit
{
    public class EventHadlers
    {
        private static Unit _lastSpawned;
        
        public void OnTeamChoose(RespawningTeamEventArgs ev)
        {
            if (new Random().Next(101) < 55)
            {
                if (!SpawnChance(ev.NextKnownTeam, ev.Players))
                {
                    ev.IsAllowed = !SpawnTicket(ev.NextKnownTeam, ev.Players);
                }
                else
                    ev.IsAllowed = false;
            }

            Methods.AddChance(ev);

            foreach (var un in Plugin.Tickets.Keys)
                Plugin.Tickets[un] -= un.TicketsToRemove;
        }

        public void OnShooting(ShotEventArgs ev)
        {
            if (Plugin.Soldiers.TryGetValue(ev.Player, out string name))
            {
                if (Plugin.Soldiers.TryGetValue(ev.Target, out string atname) && name == atname)
                {
                    ev.CanHurt = false;
                    return;
                }

                if (Plugin.Configs[name].AllowToDamage.Contains(ev.Target.Role.Team))
                    ev.CanHurt = true;
            }
            else if (Plugin.Soldiers.TryGetValue(ev.Target, out string name1))
            {
                if (Plugin.Configs[name1].AllowToDamage.Contains(ev.Player.Role.Team))
                    ev.CanHurt = true;
            }
        }

        public void OnDied(DiedEventArgs ev)
        {
            if (Plugin.Soldiers.Keys.Contains(ev.Player))
                Plugin.Soldiers.Remove(ev.Player);

            Methods.AddChance(ev);
        }

        private static bool SpawnChance(SpawnableTeamType team, List<Player> players)
        {
            Random rn = new();

            foreach (var un in Plugin.Chance.Where(x => x.Team == team || x.Team == SpawnableTeamType.None))
            {
                if (un.SpawnChance > rn.Next(101))
                {
                    Timing.CallDelayed(0.5f, () =>
                    {
                        Spawn(un, players, team);
                    });

                    return true;
                }
            }

            return false;
        }

        private static bool SpawnTicket(SpawnableTeamType team, List<Player> players)
        {
            int tick = 0;
            if (team == SpawnableTeamType.ChaosInsurgency) tick = (int)Respawn.ChaosTickets;
            else if (team == SpawnableTeamType.NineTailedFox) tick = (int)Respawn.NtfTickets;

            foreach (var conf in Plugin.Tickets.Where(x => x.Key.Team == team || x.Key.Team == SpawnableTeamType.None))
            {
                if (conf.Key.Team == SpawnableTeamType.None)
                {
                    Log.Error("None teams can use only chance system!");
                    continue;
                }
                
                if (conf.Value < tick)
                    continue;

                Timing.CallDelayed(0.5f, () =>
                {
                    Spawn(conf.Key, players, team);
                    Plugin.Tickets[conf.Key] -= conf.Key.TicketsToRemove;
                });

                return true;
            }

            return false;
        }

        public static void Spawn(Unit un, List<Player> players, SpawnableTeamType team)
        {
            foreach (Player player in players)
            {
                var pos = team == SpawnableTeamType.NineTailedFox
                    ? PlayerRoles.RoleTypeId.NtfCaptain.GetRandomSpawnLocation().Position
                    : PlayerRoles.RoleTypeId.ChaosConscript.GetRandomSpawnLocation().Position;

                var list = new List<SpawnPosition>();
                list.AddRange(un.StaticSpawnPoints.Select(stpos => new SpawnPosition(stpos.Position, stpos.Chance)).ToList());
                list.AddRange(un.DynamicSpawnPoints.Select(dnpos => new SpawnPosition(dnpos.Position, dnpos.Chance)));
                list.AddRange(un.RoomSpawnPoints.Select(rmpos => new SpawnPosition(rmpos.RoomName.GetSpawnPos(), rmpos.Chance)));

                for (int i = 0; i < 15; i++)
                {
                    var it = list.RandomItem();
                    if (new Random().Next(101) <= it.Chance)
                        pos = it.Position;
                }

                player.Role.Set(un.Roles.ToList().RandomItem(), SpawnReason.Respawn);
                player.Position = pos;

                if (un.OverrideInventory)
                    player.ClearInventory();

                foreach (ItemType item in un.Inventory)
                    player.AddItem(item);
                foreach (KeyValuePair<AmmoType, ushort> ammo in un.Ammos)
                    player.AddAmmo(ammo.Key, ammo.Value);

                if (un.AssignCustomInfo)
                {
                    player.CustomInfo = un.CustomInfo.Replace("%name%", un.UnitName);
                    player.InfoArea = PlayerInfoArea.Nickname & PlayerInfoArea.CustomInfo & PlayerInfoArea.Badge;
                }

                Plugin.Soldiers.Add(player, un.UnitName);
            }

            Cassie.Message(un.CassieText.Replace("%name%", un.UnitName), isSubtitles: un.Subtiteled);

            _lastSpawned = un;
        }
    }
}
