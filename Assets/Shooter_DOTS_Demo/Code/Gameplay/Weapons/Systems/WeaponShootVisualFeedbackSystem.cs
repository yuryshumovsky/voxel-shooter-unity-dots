using Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(WeaponGenerateProjectileSystem))]
    public partial struct WeaponShootVisualFeedbackSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            WeaponVisualFeedbackJob weaponVisualFeedbackJob = new WeaponVisualFeedbackJob
            {
                DeltaTime = deltaTime,
                LocalTransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(false),
            };
            state.Dependency = weaponVisualFeedbackJob.Schedule(state.Dependency);
        }

        [BurstCompile]
        [WithAll(typeof(Simulate))]
        public partial struct WeaponVisualFeedbackJob : IJobEntity
        {
            public float DeltaTime;
            public ComponentLookup<LocalTransform> LocalTransformLookup;

            void Execute(
                Entity weaponEntity,
                ref BaseWeaponComponent baseWeapon,
                ref WeaponVisualFeedbackComponent weaponFeedback)
            {
                if (baseWeapon.LastVisualTotalShotsCountInitialized == 0)
                {
                    baseWeapon.LastVisualTotalShotsCount = baseWeapon.TotalShotsCount;
                    baseWeapon.LastVisualTotalShotsCountInitialized = 1;
                }

                uint shotsSinceLastFrame = baseWeapon.TotalShotsCount - baseWeapon.LastVisualTotalShotsCount;

                if (shotsSinceLastFrame > 0)
                {
                    weaponFeedback.CurrentRecoil += weaponFeedback.RecoilStrength * shotsSinceLastFrame;
                }

                if (!LocalTransformLookup.HasComponent(weaponEntity))
                {
                    baseWeapon.LastVisualTotalShotsCount = baseWeapon.TotalShotsCount;
                    return;
                }

                ref LocalTransform weaponTransform = ref LocalTransformLookup.GetRefRW(weaponEntity).ValueRW;

                if (weaponFeedback.BasePositionInitialized == 0)
                {
                    weaponFeedback.BaseLocalPosition = weaponTransform.Position;
                    weaponFeedback.BasePositionInitialized = 1;
                }

                weaponFeedback.CurrentRecoil = math.clamp(
                    weaponFeedback.CurrentRecoil, 0f, weaponFeedback.RecoilMaxDistance);

                float3 forwardDirection = math.forward(weaponTransform.Rotation);
                float3 targetRecoilPosition = forwardDirection * -weaponFeedback.CurrentRecoil;
                float currentRecoilMagnitude = math.length(weaponFeedback.WeaponLocalPosRecoil);
                float targetRecoilMagnitude = weaponFeedback.CurrentRecoil;

                if (currentRecoilMagnitude < targetRecoilMagnitude * 0.99f)
                {
                    weaponFeedback.WeaponLocalPosRecoil = math.lerp(
                        weaponFeedback.WeaponLocalPosRecoil,
                        targetRecoilPosition,
                        math.saturate(weaponFeedback.RecoilSharpness * DeltaTime));
                }
                else
                {
                    weaponFeedback.WeaponLocalPosRecoil = math.lerp(
                        weaponFeedback.WeaponLocalPosRecoil, 
                        float3.zero,
                        math.saturate(weaponFeedback.RecoilRestitutionSharpness * DeltaTime));
                    float newRecoilMagnitude = math.length(weaponFeedback.WeaponLocalPosRecoil);
                    weaponFeedback.CurrentRecoil = newRecoilMagnitude;
                }

                weaponTransform.Position = weaponFeedback.BaseLocalPosition + weaponFeedback.WeaponLocalPosRecoil;

                baseWeapon.LastVisualTotalShotsCount = baseWeapon.TotalShotsCount;
            }
        }
    }
}

