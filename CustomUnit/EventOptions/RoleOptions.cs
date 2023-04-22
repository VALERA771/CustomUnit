using PlayerRoles;
using PluginAPI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace CustomUnit.EventOptions
{
    public struct RoleOptions : Options.IOption<RoleTypeId>
    {
        public RoleOptions(ServerEventType type)
        {
            EventType = type;
        }

        public bool IsEnabled { get; set; } = true;
        public int Chance { get; set; } = 1;

        [YamlIgnore]
        public ServerEventType EventType { get; }

        public HashSet<RoleTypeId> Allow { get; set; } = new();
        public HashSet<RoleTypeId> Disallow { get; set; } = new();
    }
}
