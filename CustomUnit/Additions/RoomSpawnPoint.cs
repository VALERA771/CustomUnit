using System.ComponentModel;
using MapGeneration;

namespace CustomUnit.Additions;

public struct RoomSpawnPoint
{
    public RoomSpawnPoint(RoomName roomName, int chance)
    {
        RoomName = roomName;
        Chance = chance;
    }
    
    [Description("The room name")]
    public RoomName RoomName { get; }
    
    [Description("The chance to spawn unit there")]
    public float Chance { get; }
}