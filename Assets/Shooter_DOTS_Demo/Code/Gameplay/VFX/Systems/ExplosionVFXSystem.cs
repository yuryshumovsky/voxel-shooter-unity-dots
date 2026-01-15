using Shooter_DOTS_Demo.Code.Gameplay.VFX.Components;
using Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components.Projectile;
using Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components.Specific;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Shooter_DOTS_Demo.Code.Gameplay.VFX.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct ExplosionVFXSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<VFXExplosionsManagerSingletonComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            VFXExplosionsManagerSingletonComponent vfxExplosionsManagerSingletonComponent =
                SystemAPI.GetSingletonRW<VFXExplosionsManagerSingletonComponent>().ValueRW;

            ExplosionVFXJob job = new ExplosionVFXJob
            {
                VFXExplosionsManager = vfxExplosionsManagerSingletonComponent.Manager,
            };
            state.Dependency = job.Schedule(state.Dependency);
        }

        public partial struct ExplosionVFXJob : IJobEntity
        {
            public VFXManager<VFXExplosionRequest> VFXExplosionsManager;

            void Execute(
                Entity entity,
                in LocalTransform transform,
                ref RocketComponent rocket,
                in ProjectileComponent projectile
            )
            {
                if (rocket.HasProcessedHitVFX == 0 && projectile.HasHit == 1)
                {
                    var request = new VFXExplosionRequest
                    {
                        Position = transform.Position,
                        Size = rocket.ExplosionSize,
                    };

                    VFXExplosionsManager.AddRequest(request);

                    rocket.HasProcessedHitVFX = 1;
                }
            }
        }
    }
}