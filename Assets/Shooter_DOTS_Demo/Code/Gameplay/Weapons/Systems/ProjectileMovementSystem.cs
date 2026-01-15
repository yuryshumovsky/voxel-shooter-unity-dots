using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components.Projectile;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(WeaponSpawnProjectileSystem))]
    public partial struct ProjectileMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;

            ProjectileMovementJob job = new ProjectileMovementJob
            {
                DeltaTime = deltaTime,
            };
            state.Dependency = job.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        [WithAll(typeof(Simulate))]
        public partial struct ProjectileMovementJob : IJobEntity
        {
            public float DeltaTime;

            void Execute(ref ProjectileComponent projectileComponent, ref LocalTransform localTransform)
            {
                if (projectileComponent.HasHit != 0)
                {
                    return;
                }

                projectileComponent.Velocity += (math.up() * projectileComponent.Gravity) * DeltaTime;
                float3 displacement = projectileComponent.Velocity * DeltaTime;
                localTransform.Position += displacement;
            }
        }
    }
}