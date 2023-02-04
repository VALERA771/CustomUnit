using Exiled.API.Enums;
using Exiled.API.Interfaces;
using PlayerRoles;
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
        [Description("Whether or not plugin is enabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Whether or not debug messages should appear")]
        public bool Debug { get; set; } = false;

        [Description("Unit name")]
        public string UnitName { get; set; } = "UnitName";

        [Description("Chance.From 1 to 100")]
        public int SpawnChance { get; set; } = 10;

        [Description("Spawn team")]
        public SpawnableTeamType Team { get; set; } = SpawnableTeamType.ChaosInsurgency;

        [Description("Role")]
        public RoleTypeId Role { get; set; } = RoleTypeId.Tutorial;

        [Description("Inventory item")]
        public List<ItemType> Inventory { get; set; } = new List<ItemType>()
        {
            ItemType.Medkit,
            ItemType.KeycardChaosInsurgency,
            ItemType.GunE11SR
        };

        [Description("Inventory Ammo")]
        public Dictionary<AmmoType, ushort> Ammos = new Dictionary<AmmoType, ushort>()
        {
            { AmmoType.Nato556, 100 },
            { AmmoType.Nato762, 50 }
        };

        [Description("CASSIE announchement")]
        public string CassieText { get; set; } = "%name% has arrived!";
    }
}
