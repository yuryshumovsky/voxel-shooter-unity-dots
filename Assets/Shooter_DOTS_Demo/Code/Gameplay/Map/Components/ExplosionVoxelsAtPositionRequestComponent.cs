using Unity.Entities;
using Unity.Mathematics;

namespace Shooter_DOTS_Demo.Code.Gameplay.Map.Components
{
    public struct ExplosionVoxelsAtPositionRequestComponent : IComponentData
    {
        public float3 HitWorldPosition;
        public int ExplosionRadius;
    }
}