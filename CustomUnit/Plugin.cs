using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Server;
using PlayerRoles;
using System;
using System.Collections.Generic;
using UnityEngine;

using Random = System.Random;
using Map = Exiled.Events.Handlers.Server;
using MEC;
using Exiled.API.Features.Roles;

namespace CustomUnit
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance { get; private set; }

        public override string Name => "Custom Unit";
        public override string Prefix => "CustomUnit";
        public override string Author => "VALERA771#1471";
        public override Version Version => new Version(1, 0, 0);
        public override Version RequiredExiledVersion => new Version(6, 0, 0);

        public override void OnEnabled()
        {
            Instance = this;
            Map.RespawningTeam += OnTeamChoose;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Instance = null;
            Map.RespawningTeam -= OnTeamChoose;
            base.OnDisabled();
        }

        public override void OnReloaded()
        {
            base.OnReloaded();
        }

        public Plugin()
        {
        }


        public void OnTeamChoose(RespawningTeamEventArgs ev)
        {
            if (ev.NextKnownTeam == Instance.Config.Team)
            {
                Random rn = new Random();
                int ch = rn.Next(0, 101);
                Log.Info(ch);
                if (Instance.Config.SpawnChance > ch)
                {
                    Timing.CallDelayed(0.5f, () =>
                    {
                        foreach (Player player in ev.Players)
                        {
                            SpawnLocation loc;
                            if (ev.NextKnownTeam == Respawning.SpawnableTeamType.NineTailedFox) loc = RoleTypeId.NtfCaptain.GetRandomSpawnLocation();
                            else loc = RoleTypeId.ChaosConscript.GetRandomSpawnLocation();

                            Vector3 pos = loc.Position;

                            player.Role.Set(Instance.Config.Role, SpawnReason.Respawn);
                            player.Position = pos;
                            player.ClearInventory();
                            foreach (ItemType item in Instance.Config.Inventory)
                                player.AddItem(item);
                            foreach (KeyValuePair<AmmoType, ushort> ammo in Instance.Config.Ammos)
                                player.AddAmmo(ammo.Key, ammo.Value);
                            player.CustomInfo = Instance.Config.UnitName + " solder";
                            player.InfoArea = PlayerInfoArea.Nickname & PlayerInfoArea.CustomInfo & PlayerInfoArea.Badge;
                        }

                        Cassie.Message(Instance.Config.CassieText.Replace("%name%", Instance.Config.UnitName), isSubtitles: true);
                    });
                }
            }
        }
    }
}
