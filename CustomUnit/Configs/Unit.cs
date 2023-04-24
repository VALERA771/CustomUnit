using Exiled.API.Enums;
using PlayerRoles;
using PluginAPI.Enums;
using Respawning;
using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Features.Spawn;

namespace CustomUnit.Configs
{
    public sealed class Unit
    {
        [Description("Uses chance system. If \"false\" will use ticket system")]
        public bool UseChance { get; set; } = true;

        [Description("Unit name")]
        public string UnitName { get; set; } = "UnitName";

        [Description("Chance. From 1 to 100")]
        public int SpawnChance { get; set; } = 10;

        [Description("Spawn team")]
        public SpawnableTeamType Team { get; set; } = SpawnableTeamType.ChaosInsurgency;

        [Description("Roles. Picks up a random role for each player in unit")]
        public HashSet<RoleTypeId> Roles { get; set; } = new()
        {
            RoleTypeId.Tutorial,
            RoleTypeId.Scp939
        };

        [Description("List of static spawn points (Depends on rooms). Leave null to use \"team\" spawnpoint")]
        public List<DynamicSpawnPoint> DynamicSpawnPoints { get; set; } = new()
        {
            new()
            {
                Chance = 100,
                Location = SpawnLocationType.Inside914,
            }
        };

        [Description("List of static spawn points (Depends on coordinates). Leave null to use \"team\" spawnpoint")]
        public List<StaticSpawnPoint> StaticSpawnPoints { get; set; } = new();

        [Description(
            "Should players on spawn have their default inventories? (If \"false\" items from inventory will be just added otherwise they'll replace defualt items)")]
        public bool OverrideInventory { get; set; } = true;

        [Description("Inventory item")]
        public List<ItemType> Inventory { get; set; } = new()
        {
            ItemType.Medkit,
            ItemType.KeycardChaosInsurgency,
            ItemType.GunE11SR
        };

        [Description("Inventory Ammo")]
        public Dictionary<AmmoType, ushort> Ammos { get; set; } = new()
        {
            { AmmoType.Nato556, 100 },
            { AmmoType.Nato9, 50 }
        };

        [Description("CASSIE announchement. Replaces %name% with unit_name")]
        public string CassieText { get; set; } = "%name% has arrived!";

        [Description("Should CASSIE message have subtiteles?")]
        public bool Subtiteled { get; set; } = true;

        [Description("Teams that unit can damage")]
        public List<Team> AllowToDamage { get; set; } = new()
        {
            PlayerRoles.Team.SCPs
        };

        [Description("Events to add tickets")]
        public Dictionary<ServerEventType, int> Events { get; set; } = new()
        {
            [ServerEventType.WarheadDetonation] = -1,
            [ServerEventType.PlayerDying] = 1
        };

        [Description("Tickets to remove if team spawns (Also this value will be removed if remove_ticket_on_other is true)")]
        public int TicketsToRemove { get; set; } = 30;

        [Description("Should tickets be removed if other team spawned?")]
        public bool RemoveTicketOnOther { get; set; } = true;

        [Description("Amount of ticket which team will have on start of the round")]
        public int StartTicket { get; set; } = 0;

        public override string ToString() => $"Name={UnitName} Team={Team}";
    }
}
