using PlayerRoles;
using System.Collections.Generic;

namespace CustomUnit.EventOptions
{
    public struct RoleOptions
    {
        public RoleOptions()
        {
        }

        public bool IsEnabled { get; set; } = true;
        public int Chance { get; set; } = 100;

        public HashSet<RoleTypeId> Allow { get; set; } = new();
        public HashSet<RoleTypeId> Disallow { get; set; } = new();
    }
}
