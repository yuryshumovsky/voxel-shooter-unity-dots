using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components.Projectile;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(TransformSystemGroup))]
    [UpdateBefore(typeof(LocalToWorldSystem))]
    public partial struct PrefabProjectileVisualsSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            PrefabProjectileVisualsJob job = new PrefabProjectileVisualsJob();
            state.Dependency = job.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        public partial struct PrefabProjectileVisualsJob : IJobEntity
        {
            void Execute(ref LocalToWorld ltw, in LocalTransform transform, in ProjectileComponent projectile)
            {
                float3 visualOffset = math.lerp(
                    projectile.VisualOffset,
                    float3.zero,
                    math.saturate(projectile.LifetimeCounter / projectile.VisualOffsetCorrectionDuration)
                );

                float4x4 visualOffsetTransform = float4x4.Translate(visualOffset);
                ltw.Value = math.mul(
                    visualOffsetTransform,
                    float4x4.TRS(
                        transform.Position,
                        quaternion.LookRotationSafe(math.normalizesafe(projectile.Velocity), math.up()),
                        transform.Scale
                    )
                );
            }
        }
    }
}