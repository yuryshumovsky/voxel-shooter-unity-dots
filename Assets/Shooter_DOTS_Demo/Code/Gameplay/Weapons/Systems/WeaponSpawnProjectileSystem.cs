using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components;
using Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components.Projectile;
using Unity.Collections;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(WeaponGenerateProjectileSystem))]
    public partial struct WeaponSpawnProjectileSystem : ISystem
    {
        private ComponentLookup<ProjectileComponent> _projectileComponentLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            _projectileComponentLookup = state.GetComponentLookup<ProjectileComponent>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _projectileComponentLookup.Update(ref state);

            EntityCommandBuffer ecb = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            WeaponSpawnProjectileJob weaponSpawnProjectileJob = new WeaponSpawnProjectileJob
            {
                ECB = ecb,
                ProjectileLookup = _projectileComponentLookup
            };
            weaponSpawnProjectileJob.ProjectileLookup.Update(ref state);
            state.Dependency = weaponSpawnProjectileJob.Schedule(state.Dependency);
        }

        [BurstCompile]
        [WithAll(typeof(Simulate))]
        public partial struct WeaponSpawnProjectileJob : IJobEntity
        {
            public EntityCommandBuffer ECB;
            [ReadOnly] public ComponentLookup<ProjectileComponent> ProjectileLookup;

            void Execute(
                Entity weaponEntity,
                in WeaponProjectileReferenceComponent weaponProjectileReferenceComponent,
                in DynamicBuffer<WeaponProjectileWaitToSpawnBufferData> projectileBuffer,
                in LocalTransform weaponLocalTransform,
                in DynamicBuffer<WeaponShotIgnoredEntityBufferData> ignoredEntities
            )
            {
                if (!ProjectileLookup.TryGetComponent(
                        weaponProjectileReferenceComponent.ProjectilePrefab,
                        out ProjectileComponent projectileComponent
                    ))
                {
                    return;
                }

                for (int i = 0; i < projectileBuffer.Length; i++)
                {
                    WeaponProjectileWaitToSpawnBufferData projectileBufferData = projectileBuffer[i];

                    Entity spawnedProjectileEntity = ECB.Instantiate(weaponProjectileReferenceComponent.ProjectilePrefab);
                    ECB.SetName(spawnedProjectileEntity, (FixedString64Bytes)$"projectile_{projectileBufferData.ID}");
                    LocalTransform localTransform = LocalTransform.FromPositionRotation(
                        projectileBufferData.SimulationPosition,
                        projectileBufferData.SimulationRotation
                    );

                    ECB.SetComponent(spawnedProjectileEntity, localTransform);

                    ECB.SetComponent(
                        spawnedProjectileEntity,
                        new ProjectileSpawnIdComponent
                            { WeaponEntity = weaponEntity, SpawnId = projectileBufferData.ID }
                    );

                    for (int k = 0; k < ignoredEntities.Length; k++)
                    {
                        ECB.AppendToBuffer(spawnedProjectileEntity, ignoredEntities[k]);
                    }

                    projectileComponent.Velocity = projectileComponent.Speed * projectileBufferData.SimulationDirection;
                    projectileComponent.VisualOffset =
                        projectileBufferData.VisualPosition - projectileBufferData.SimulationPosition;
                    projectileComponent.SpawnPosition = projectileBufferData.SimulationPosition;

                    ECB.SetComponent(spawnedProjectileEntity, projectileComponent);
                }
            }
        }
    }
}