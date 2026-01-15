using Unity.Burst;
using Unity.Entities;
using Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components.Projectile;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ProjectileMovementSystem))]
    public partial struct ProjectileLifetimeSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;

            var ecb = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

            ProjectileLifetimeJob projectileLifetimeJob = new ProjectileLifetimeJob
            {
                DeltaTime = deltaTime,
                ECB = ecb,
            };
            state.Dependency = projectileLifetimeJob.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        [WithAll(typeof(Simulate))]
        public partial struct ProjectileLifetimeJob : IJobEntity
        {
            public float DeltaTime;
            public EntityCommandBuffer.ParallelWriter ECB;

            void Execute(Entity entity, [ChunkIndexInQuery] int chunkIndex, ref ProjectileComponent projectileComponent)
            {
                if (projectileComponent.HasHit != 0)
                {
                    return;
                }

                projectileComponent.LifetimeCounter += DeltaTime;

                if (projectileComponent.LifetimeCounter >= projectileComponent.MaxLifetime)
                {
                    ECB.DestroyEntity(chunkIndex, entity);
                }
            }
        }
    }
}