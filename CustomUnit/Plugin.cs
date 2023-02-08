using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Server;
using System;
using System.Collections.Generic;
using UnityEngine;

using Random = System.Random;
using Map = Exiled.Events.Handlers.Server;
using MEC;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using System.Linq;

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
            _event = new EventHadlers();
            Instance = this;
            Map.RespawningTeam += _event.OnTeamChoose;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            _event = null;
            Instance = null;
            Map.RespawningTeam -= _event.OnTeamChoose;
            base.OnDisabled();
        }

        public override void OnReloaded()
        {
            base.OnReloaded();
        }

        public Plugin()
        {
        }

        public static HashSet<Player> Soldiers = new HashSet<Player>();
        public static EventHadlers _event;
        public static int Tickets { get; set; } = 0;
    }
}
