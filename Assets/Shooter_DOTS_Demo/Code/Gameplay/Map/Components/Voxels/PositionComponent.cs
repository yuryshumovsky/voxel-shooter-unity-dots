using Unity.Entities;
using Unity.Mathematics;

namespace Shooter_DOTS_Demo.Code.Gameplay.Map.Components.Voxels
{
    public struct PositionComponent : IComponentData
    {
        public int3 Value;
    }
}

