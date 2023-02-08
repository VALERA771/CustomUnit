using Exiled.API.Features;
using System;
using System.Collections.Generic;

using Map = Exiled.Events.Handlers.Server;
using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;

namespace CustomUnit
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance { get; private set; }

        public override string Name => "Custom Unit";
        public override string Prefix => "CustomUnit";
        public override string Author => "VALERA771#1471";
        public override Version Version => new Version(1, 2, 0);
        public override Version RequiredExiledVersion => new Version(6, 0, 0);

        public override void OnEnabled()
        {
            _event = new EventHadlers();
            Instance = this;
            Map.RespawningTeam += _event.OnTeamChoose;
            Player.Died += _event.OnDied;
            Player.Shot += _event.OnShooting;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            _event = null;
            Instance = null;
            Map.RespawningTeam -= _event.OnTeamChoose;
            Player.Died -= _event.OnDied;
            Player.Shot -= _event.OnShooting;
            base.OnDisabled();
        }

        public override void OnReloaded()
        {
            base.OnReloaded();
        }

        public Plugin()
        {
        }

        public static HashSet<Exiled.API.Features.Player> Soldiers = new HashSet<Exiled.API.Features.Player>();
        public static EventHadlers _event;
        public static int Tickets { get; set; } = 0;
    }
}
