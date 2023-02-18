using CustomUnit.Configs;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using Respawning;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = System.Random;

namespace CustomUnit
{
    public class EventHadlers
    {
        public void OnTeamChoose(RespawningTeamEventArgs ev)
        {
            if (!SpawnChance(ev.NextKnownTeam, ev.Players)) SpawnTicket(ev.NextKnownTeam, ev.Players);
        }

        public void OnShooting(ShotEventArgs ev)
        {
            if (Plugin.Soldiers.TryGetValue(ev.Player, out string name))
            {
                if (Plugin.Soldiers.TryGetValue(ev.Target, out string atname))
                {
                    if (name == atname)
                    {
                        ev.CanHurt = false;
                        return;
                    }
                    else
                    {
                        ev.CanHurt = true;
                        return;
                    }
                }

                if (Plugin.Configs[name].AllowToDamage.Contains(ev.Target.Role.Team))
                    ev.CanHurt = true;
            }
            else if (Plugin.Soldiers.TryGetValue(ev.Target, out string name1))
            {
                if (Plugin.Configs[name1].AllowToDamage.Contains(ev.Player.Role.Team))
                    ev.CanHurt = true; return;
            }
        }

        public void OnDied(DiedEventArgs ev)
        {
            if (Plugin.Soldiers.Keys.Contains(ev.Player))
                Plugin.Soldiers.Remove(ev.Player);
        }

        public static bool SpawnChance(SpawnableTeamType team, List<Player> players)
        {
            Random rn = new Random();

            foreach (var un in Plugin.Chance)
            {
                if (un.SpawnChance > rn.Next(101))
                {
                    if (un.Team != team)
                        continue;

                    Timing.CallDelayed(0.5f, () =>
                    {
                        Spawn(un, players, team);
                    });

                    return true;
                }
            }

            return false;
        }

        public static bool SpawnTicket(SpawnableTeamType team, List<Player> players)
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
                });

                return true;
            }

            return false;
        }

        private static void Spawn(Unit un, List<Player> players, SpawnableTeamType team)
        {
            foreach (Player player in players)
            {
                SpawnLocation loc;
                if (team == SpawnableTeamType.NineTailedFox) loc = PlayerRoles.RoleTypeId.NtfCaptain.GetRandomSpawnLocation();
                else loc = PlayerRoles.RoleTypeId.ChaosConscript.GetRandomSpawnLocation();

                Vector3 pos = loc.Position;

                player.Role.Set(un.Roles.ToList().RandomItem(), SpawnReason.Respawn);
                player.Position = pos;
                player.ClearInventory();
                foreach (ItemType item in un.Inventory)
                    player.AddItem(item);
                foreach (KeyValuePair<AmmoType, ushort> ammo in un.Ammos)
                    player.AddAmmo(ammo.Key, ammo.Value);
                player.CustomInfo = un.UnitName + " solder";
                player.InfoArea = PlayerInfoArea.Nickname & PlayerInfoArea.CustomInfo & PlayerInfoArea.Badge;

                Plugin.Soldiers.Add(player, un.UnitName);
            }

            Cassie.Message(un.CassieText.Replace("%name%", un.UnitName), isSubtitles: true);
        }
    }
}
