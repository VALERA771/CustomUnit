using PluginAPI.Enums;
using System;
using System.Collections.Generic;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp914;
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
            [typeof(DetonatingEventArgs)] = ServerEventType.WarheadDetonation,
            [typeof(SpawningRagdollEventArgs)] = ServerEventType.RagdollSpawn,
            [typeof(AnnouncingScpTerminationEventArgs)] = ServerEventType.CassieAnnouncesScpTermination,
            [typeof(DecontaminatingEventArgs)] = ServerEventType.LczDecontaminationStart,
            [typeof(PlacingBloodEventArgs)] = ServerEventType.PlaceBlood,
            [typeof(ChangingRoleEventArgs)] = ServerEventType.PlayerChangeRole,
            [typeof(UpgradingInventoryItemEventArgs)] = ServerEventType.Scp914UpgradeInventory
        };

        public interface IOption<T>
        {
            public bool IsEnabled { get; set; }
            public int Chance { get; set; }

            public HashSet<T> Allow { get; set; }
            public HashSet<T> Disallow { get; set; }
        }
    }
}
