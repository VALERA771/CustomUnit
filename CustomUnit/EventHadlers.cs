using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp330;
using Exiled.Events.EventArgs.Server;
using MEC;
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
            if (ev.NextKnownTeam == Plugin.Instance.Config.Team)
            {
                /*if (Plugin.Instance.Config.UseChance)
                {*/
                    Random rn = new Random();
                    int ch = rn.Next(0, 101);
                    Log.Info(ch);
                    if (Plugin.Instance.Config.SpawnChance > ch)
                    {
                        Timing.CallDelayed(0.5f, () =>
                        {
                            foreach (Player player in ev.Players)
                            {
                                SpawnLocation loc;
                                if (ev.NextKnownTeam == Respawning.SpawnableTeamType.NineTailedFox) loc = PlayerRoles.RoleTypeId.NtfCaptain.GetRandomSpawnLocation();
                                else loc = PlayerRoles.RoleTypeId.ChaosConscript.GetRandomSpawnLocation();

                                Vector3 pos = loc.Position;

                                player.Role.Set(Plugin.Instance.Config.Roles.ToList().RandomItem(), SpawnReason.Respawn);
                                player.Position = pos;
                                player.ClearInventory();
                                foreach (ItemType item in Plugin.Instance.Config.Inventory)
                                    player.AddItem(item);
                                foreach (KeyValuePair<AmmoType, ushort> ammo in Plugin.Instance.Config.Ammos)
                                    player.AddAmmo(ammo.Key, ammo.Value);
                                player.CustomInfo = Plugin.Instance.Config.UnitName + " solder";
                                player.InfoArea = PlayerInfoArea.Nickname & PlayerInfoArea.CustomInfo & PlayerInfoArea.Badge;

                                Plugin.Soldiers.Add(player);
                            }

                            Cassie.Message(Plugin.Instance.Config.CassieText.Replace("%name%", Plugin.Instance.Config.UnitName), isSubtitles: true);
                        });
                    }
                    /*else
                    {
                        bool spawn = (Plugin.Instance.Config.Team == Respawning.SpawnableTeamType.NineTailedFox && Respawn.NtfTickets < Plugin.Tickets) || (Plugin.Instance.Config.Team == Respawning.SpawnableTeamType.ChaosInsurgency && Respawn.ChaosTickets < Plugin.Tickets);
                        if (spawn)
                        {
                            Plugin.Tickets -= Plugin.Instance.Config.TicketsToRemove;
                            if (Plugin.Tickets < 0)
                                Plugin.Tickets = 0;

                            Timing.CallDelayed(0.5f, () =>
                            {
                                foreach (Player player in ev.Players)
                                {
                                    SpawnLocation loc;
                                    if (ev.NextKnownTeam == Respawning.SpawnableTeamType.NineTailedFox) loc = PlayerRoles.RoleTypeId.NtfCaptain.GetRandomSpawnLocation();
                                    else loc = PlayerRoles.RoleTypeId.ChaosConscript.GetRandomSpawnLocation();

                                    Vector3 pos = loc.Position;

                                    player.Role.Set(Plugin.Instance.Config.Roles.ToList().RandomItem(), SpawnReason.Respawn);
                                    player.Position = pos;
                                    player.ClearInventory();
                                    foreach (ItemType item in Plugin.Instance.Config.Inventory)
                                        player.AddItem(item);
                                    foreach (KeyValuePair<AmmoType, ushort> ammo in Plugin.Instance.Config.Ammos)
                                        player.AddAmmo(ammo.Key, ammo.Value);
                                    player.CustomInfo = Plugin.Instance.Config.UnitName + " solder";
                                    player.InfoArea = PlayerInfoArea.Nickname & PlayerInfoArea.CustomInfo & PlayerInfoArea.Badge;

                                    Plugin.Soldiers.Add(player);
                                }

                                Cassie.Message(Plugin.Instance.Config.CassieText.Replace("%name%", Plugin.Instance.Config.UnitName), isSubtitles: true);
                            });
                        }*/
                    //}
                //}
            }
        }

        public void OnShooting(ShotEventArgs ev)
        {
            if (Plugin.Soldiers.Contains(ev.Player))
            {
                if (Plugin.Soldiers.Contains(ev.Target))
                    ev.CanHurt = false;
                else if (Plugin.Instance.Config.AllowToDamage.Contains(ev.Target.Role.Team))
                    ev.CanHurt = true;
            }
        }

        public void OnDied(DiedEventArgs ev)
        {
            if (Plugin.Soldiers.Contains(ev.Player))
                Plugin.Soldiers.Remove(ev.Player);
        }

        /*public void OnScp330Pick(InteractingScp330EventArgs ev)
        {
            if (Plugin.Instance.Config.Events.TryGetValue(PluginAPI.Enums.ServerEventType.PlayerInteractScp330, out int chance))
                Plugin.Tickets += chance;
        }

        public void OnCoinFlip(FlippingCoinEventArgs ev)
        {
            if (Plugin.Instance.Config.Events.TryGetValue(PluginAPI.Enums.ServerEventType.PlayerCoinFlip, out int chance))
                Plugin.Tickets += chance;
        }*/
    }
}
