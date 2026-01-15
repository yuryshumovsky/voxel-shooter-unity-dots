using Unity.Entities;
using Unity.Mathematics;

namespace Shooter_DOTS_Demo.Code.Gameplay.Map.Components
{
    public struct MeshBoxSpawnDataBufferElement : IBufferElementData
    {
        public byte VoxelType;
        public int3 PositionInChunk;
        public int3 PositionInWorld;
        public int3 Size;
        public float4 Color;
        public Entity ChunkEntity;
    }
}