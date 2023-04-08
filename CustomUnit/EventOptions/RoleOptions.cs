using PlayerRoles;
using PluginAPI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomUnit.EventOptions
{
    public struct RoleOptions
    {
        public RoleOptions(ServerEventType eventType)
        {
            EventType = eventType;
        }

        public bool IsEnabled { get; set; } = true;
        public int Chance { get; set; } = 1;

        public ServerEventType EventType { get; }

        public List<RoleTypeId> Allow { get; set; } = new();
    }
}
