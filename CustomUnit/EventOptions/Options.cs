using PluginAPI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.Events.EventArgs.Interfaces;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.EventArgs.Warhead;
using Exiled.Loader;

namespace CustomUnit.EventOptions
{
    public class Options
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
