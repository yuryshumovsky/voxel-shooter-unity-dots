using Unity.Entities;
using Unity.Mathematics;

namespace Shooter_DOTS_Demo.Code.Gameplay.Map.Components
{
    public struct VoxelBufferElement : IBufferElementData
    {
        public byte VoxelType;
        public int3 DebugPosition;
    }

    public struct VoxelToDestroyBufferElement : IBufferElementData
    {
        public Entity Chunk;
    }
}