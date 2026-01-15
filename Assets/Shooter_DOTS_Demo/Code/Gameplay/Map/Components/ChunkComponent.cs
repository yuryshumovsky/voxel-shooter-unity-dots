using Unity.Entities;
using Unity.Mathematics;

namespace Shooter_DOTS_Demo.Code.Gameplay.Map.Components
{
    public struct ChunkComponent : IComponentData
    {
        public int3 ChunkPosition;
        public int ChunkSize;
    }
}