using Exiled.API.Enums;
using Exiled.API.Interfaces;
using PlayerRoles;
using PluginAPI.Enums;
using Respawning;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomUnit
{
    public sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        /*[Description("Uses chance system. If \"false\" will use ticket system")]
        public bool UseChance { get; set; } = true;*/

        [Description("Unit name")]
        public string UnitName { get; set; } = "UnitName";

        [Description("Chance. From 1 to 100")]
        public int SpawnChance { get; set; } = 10;

        [Description("Spawn team")]
        public SpawnableTeamType Team { get; set; } = SpawnableTeamType.ChaosInsurgency;

        [Description("Roles. Picks up a random role for each player in unit")]
        public HashSet<RoleTypeId> Roles { get; set; } = new HashSet<RoleTypeId>
        {
            RoleTypeId.Tutorial,
            RoleTypeId.Scp939
        };

        [Description("Inventory item")]
        public List<ItemType> Inventory { get; set; } = new List<ItemType>()
        {
            ItemType.Medkit,
            ItemType.KeycardChaosInsurgency,
            ItemType.GunE11SR
        };

        [Description("Inventory Ammo")]
        public Dictionary<AmmoType, ushort> Ammos { get; set; } = new Dictionary<AmmoType, ushort>()
        {
            { AmmoType.Nato556, 100 },
            { AmmoType.Nato9, 50 }
        };

        [Description("CASSIE announchement. Replace %name% with unit_name")]
        public string CassieText { get; set; } = "%name% has arrived!";

        [Description("Teams that unit can damage")]
        public List<Team> AllowToDamage { get; set; } = new List<Team>
        {
            PlayerRoles.Team.SCPs
        };

        /*[Description("Events to add tickets")]
        public Dictionary<ServerEventType, int> Events { get; set; } = new Dictionary<ServerEventType, int>
        {
            { ServerEventType.PlayerPickupScp330, 1},
            { ServerEventType.PlayerCoinFlip, -1},
            { ServerEventType.}
        };

        [Description("Tickets to remove if team spawns")]
        public int TicketsToRemove { get; set; } = 30;*/
    }
}
