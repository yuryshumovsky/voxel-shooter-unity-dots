using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components.Projectile
{
    [WriteGroup(typeof(LocalToWorld))]
    public struct ProjectileComponent : IComponentData
    {
        public float Speed;
        public float Gravity;
        public float MaxLifetime;
        public float VisualOffsetCorrectionDuration;
        public float DamageRadius;

        public float3 Velocity;
        public float LifetimeCounter;
        public byte HasHit;
        public Entity HitEntity;
        public float3 VisualOffset;
        public float3 SpawnPosition;
    }
}