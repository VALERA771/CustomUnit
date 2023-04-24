using PlayerRoles;
using System.Collections.Generic;

namespace CustomUnit.EventOptions
{
    public struct RoleOptions : Options.IOption<RoleTypeId>
    {
        public RoleOptions()
        {
        }

        public bool IsEnabled { get; set; } = true;
        public int Chance { get; set; } = 1;

        public HashSet<RoleTypeId> Allow { get; set; } = new();
        public HashSet<RoleTypeId> Disallow { get; set; } = new();
    }
}
