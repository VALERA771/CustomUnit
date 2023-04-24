using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using System.IO;
using CustomUnit.EventOptions;
using PlayerRoles;
using PluginAPI.Enums;

namespace CustomUnit.Configs
{
    public sealed class Plugin : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("Path to folder with units")]
        public string UnitPath { get; set; } = Path.Combine(Paths.Exiled, "Units");

        [Description("Options for events. You can set only 1 option per event")]
        public Dictionary<ServerEventType, RoleOptions> Options { get; set; } = new()
        {
            [ServerEventType.PlayerDying] = new()
            {
                Allow = new()
                {
                    RoleTypeId.ChaosConscript,
                    RoleTypeId.ClassD
                },
                Chance = 100,
                Disallow = new()
                {
                    RoleTypeId.FacilityGuard,
                    RoleTypeId.NtfPrivate
                },
                IsEnabled = true
            }
        };
    }
}
