using Shooter_DOTS_Demo.Code.CharacterController.Components;
using Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Systems
{
    [BurstCompile]
    public partial struct ActiveWeaponSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer entityCommandBuffer = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            ActiveWeaponSetupJob activeWeaponSetupJob = new ActiveWeaponSetupJob
            {
                ECB = entityCommandBuffer,

                WeaponControlLookup = SystemAPI.GetComponentLookup<WeaponControlComponent>(true),
                FirstPersonCharacterComponentLookup = SystemAPI.GetComponentLookup<CharacterComponent>(true),
                WeaponStartPointLookup = SystemAPI
                    .GetComponentLookup<WeaponShotStartPointComponent>(false),

                LinkedEntityGroupLookup = SystemAPI.GetBufferLookup<LinkedEntityGroup>(false),
                WeaponShotIgnoredEntityLookup = SystemAPI.GetBufferLookup<WeaponShotIgnoredEntityBufferData>(false),
            };

            activeWeaponSetupJob.WeaponControlLookup.Update(ref state);
            activeWeaponSetupJob.FirstPersonCharacterComponentLookup.Update(ref state);
            activeWeaponSetupJob.WeaponStartPointLookup.Update(ref state);
            activeWeaponSetupJob.LinkedEntityGroupLookup.Update(ref state);
            activeWeaponSetupJob.WeaponShotIgnoredEntityLookup.Update(ref state);

            state.Dependency = activeWeaponSetupJob.Schedule(state.Dependency);
        }

        [BurstCompile]
        public partial struct ActiveWeaponSetupJob : IJobEntity
        {
            public EntityCommandBuffer ECB;
            [ReadOnly] public ComponentLookup<WeaponControlComponent> WeaponControlLookup;
            [ReadOnly] public ComponentLookup<CharacterComponent> FirstPersonCharacterComponentLookup;

            public ComponentLookup<WeaponShotStartPointComponent> WeaponStartPointLookup;

            public BufferLookup<LinkedEntityGroup> LinkedEntityGroupLookup;
            public BufferLookup<WeaponShotIgnoredEntityBufferData> WeaponShotIgnoredEntityLookup;

            void Execute(Entity entity, ref ActiveWeaponComponent activeWeapon)
            {
                if (activeWeapon.CurrentWeaponEntity != activeWeapon.PreviousWeaponEntity)
                {
                    if (activeWeapon.CurrentWeaponEntity == Entity.Null) return;

                    if (!WeaponControlLookup.HasComponent(activeWeapon.CurrentWeaponEntity)) return;

                    if (!FirstPersonCharacterComponentLookup.TryGetComponent(
                            entity,
                            out CharacterComponent characterComponent))
                    {
                        return;
                    }

                    if (WeaponStartPointLookup.TryGetComponent(
                            activeWeapon.CurrentWeaponEntity,
                            out WeaponShotStartPointComponent shotStartPointComponent))
                    {
                        shotStartPointComponent.Entity = characterComponent.ViewEntity;

                        WeaponStartPointLookup[activeWeapon.CurrentWeaponEntity] = shotStartPointComponent;
                    }

                    ECB.AddComponent(
                        activeWeapon.CurrentWeaponEntity,
                        new Parent { Value = characterComponent.WeaponAnimationSocketEntity }
                    );

                    ECB.SetComponent(
                        activeWeapon.CurrentWeaponEntity,
                        new WeaponOwnerComponent() { Entity = entity }
                    );

                    if (LinkedEntityGroupLookup.TryGetBuffer(
                            entity,
                            out DynamicBuffer<LinkedEntityGroup> linkedEntityBuffer))
                    {
                        linkedEntityBuffer.Add(new LinkedEntityGroup
                        {
                            Value = activeWeapon.CurrentWeaponEntity
                        });
                    }

                    if (WeaponShotIgnoredEntityLookup.TryGetBuffer(
                            activeWeapon.CurrentWeaponEntity,
                            out DynamicBuffer<WeaponShotIgnoredEntityBufferData> ignoredEntitiesBuffer))
                    {
                        ignoredEntitiesBuffer.Add(new WeaponShotIgnoredEntityBufferData() { Entity = entity });
                    }
                }

                activeWeapon.PreviousWeaponEntity = activeWeapon.CurrentWeaponEntity;
            }
        }
    }
}