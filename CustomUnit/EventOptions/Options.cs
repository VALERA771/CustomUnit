using PluginAPI.Enums;
using System;
using System.Collections.Generic;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.EventArgs.Warhead;

namespace CustomUnit.EventOptions
{
    public abstract class Options
    {
        public static Dictionary<Type, ServerEventType> Events => new()
        {
            [typeof(DiedEventArgs)] = ServerEventType.PlayerDying,
            [typeof(ActivatingGeneratorEventArgs)] = ServerEventType.GeneratorActivated,
            [typeof(StartingEventArgs)] = ServerEventType.WarheadStart,
            [typeof(DamagingShootingTargetEventArgs)] = ServerEventType.PlayerDamage,
            [typeof(EscapingEventArgs)] = ServerEventType.PlayerEscape,
            [typeof(RespawningTeamEventArgs)] = ServerEventType.TeamRespawn,
            [typeof(StoppingEventArgs)] = ServerEventType.WarheadStop,
            [typeof(DetonatingEventArgs)] = ServerEventType.WarheadDetonation
        };

        /*private static Dictionary<ServerEventType, IOption<T>> opts = new();
        public static Dictionary<ServerEventType, IOption<T>> List => opts;

        public Options(ServerEventType eventType, IOption<T> option) => opts.Add(eventType, option);*/
    }

    public interface IOption<T>
    {
        public ServerEventType EventType { get; }

        public bool IsEnabled { get; set; }
        public int Chance { get; set; }

        public List<T> Allow { get; set; }
        public List<T> Disallow { get; set; }
    }
}
