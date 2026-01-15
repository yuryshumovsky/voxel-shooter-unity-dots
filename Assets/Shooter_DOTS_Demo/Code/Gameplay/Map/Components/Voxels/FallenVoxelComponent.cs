using Unity.Entities;
using Unity.Mathematics;

namespace Shooter_DOTS_Demo.Code.Gameplay.Map.Components.Voxels
{
    public struct FallenVoxelComponent : IComponentData
    {
        public float3 Velocity;
        public float3 Gravity;
        public int3 PreviousPositionCheck;
        public int3 CurrentPosition;
    }
}