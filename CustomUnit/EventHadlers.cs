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

using Random = System.Random;

namespace CustomUnit
{
    public class EventHadlers
    {
        private static Unit _lastSpawned;
        
        public void OnTeamChoose(RespawningTeamEventArgs ev)
        {
            if (!SpawnChance(ev.NextKnownTeam, ev.Players))
            {
                ev.IsAllowed = !SpawnTicket(ev.NextKnownTeam, ev.Players);
            }
            else
                ev.IsAllowed = false;

            Methods.AddChance(ev);

            foreach (var unit in Plugin.Tickets.Keys.Where(x => x.RemoveTicketOnOther && x != _lastSpawned))
                Plugin.Tickets[unit] -= unit.TicketsToRemove;
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

            foreach (var un in Plugin.Chance.Where(x => x.Team == team))
            {
                if (un.SpawnChance < rn.Next(101))
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

            foreach (var conf in Plugin.Tickets.Where(x => x.Key.Team == team))
            {
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

                if (un.StaticSpawnPoints.Count != 0)
                {
                    var point = un.StaticSpawnPoints.RandomItem();
                    if (new Random().Next(101) >= point.Chance)
                        pos = point.Position;
                }

                if (un.DynamicSpawnPoints.Count != 0)
                {
                    var point = un.DynamicSpawnPoints.RandomItem();
                    if (new Random().Next(101) >= point.Chance)
                        pos = point.Position;
                }

                player.Role.Set(un.Roles.ToList().RandomItem(), SpawnReason.Respawn);
                player.Position = pos;

                if (un.OverrideInventory)
                    player.ClearInventory();

                foreach (ItemType item in un.Inventory)
                    player.AddItem(item);
                foreach (KeyValuePair<AmmoType, ushort> ammo in un.Ammos)
                    player.AddAmmo(ammo.Key, ammo.Value);

                player.CustomInfo = un.UnitName + " soldier";
                player.InfoArea = PlayerInfoArea.Nickname & PlayerInfoArea.CustomInfo & PlayerInfoArea.Badge;

                Plugin.Soldiers.Add(player, un.UnitName);
            }

            Cassie.Message(un.CassieText.Replace("%name%", un.UnitName), isSubtitles: un.Subtiteled);

            _lastSpawned = un;
        }
    }
}
