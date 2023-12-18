using UnityEngine;

namespace CustomUnit.Additions
{
    public struct SpawnPosition
    {
        public SpawnPosition(Vector3 position, float chance)
        {
            Position = position;
            Chance = chance;
        }
    
        public Vector3 Position { get; }
    
        public float Chance { get; }
    }
}