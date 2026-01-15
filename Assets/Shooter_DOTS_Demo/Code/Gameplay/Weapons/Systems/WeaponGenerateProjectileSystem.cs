using Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components;
using Shooter_DOTS_Demo.Code.Gameplay.Weapons.Utilities;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using TransformHelpers = Shooter_DOTS_Demo.Code.Utils.TransformHelpers;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct WeaponGenerateProjectileSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            WeaponGenerateProjectileJob weaponGenerateProjectileJob = new WeaponGenerateProjectileJob
            {
                LocalTransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true),
                ParentLookup = SystemAPI.GetComponentLookup<Parent>(true),
                PostTransformMatrixLookup = SystemAPI.GetComponentLookup<PostTransformMatrix>(true),
            };
            weaponGenerateProjectileJob.LocalTransformLookup.Update(ref state);
            weaponGenerateProjectileJob.ParentLookup.Update(ref state);
            weaponGenerateProjectileJob.PostTransformMatrixLookup.Update(ref state);
            state.Dependency = weaponGenerateProjectileJob.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        [WithAll(typeof(Simulate))]
        public partial struct WeaponGenerateProjectileJob : IJobEntity
        {
            [ReadOnly] public ComponentLookup<LocalTransform> LocalTransformLookup;
            [ReadOnly] public ComponentLookup<Parent> ParentLookup;
            [ReadOnly] public ComponentLookup<PostTransformMatrix> PostTransformMatrixLookup;

            void Execute(
                Entity _,
                ref BaseWeaponComponent baseWeapon,
                in WeaponControlComponent weaponControl,
                ref DynamicBuffer<WeaponProjectileWaitToSpawnBufferData> projectileBuffer)
            {
                projectileBuffer.Clear();

                if (!weaponControl.ShootPressed)
                {
                    return;
                }

                RigidTransform shotSimulationOrigin = WeaponUtilities.GetShotSimulationOrigin(
                    baseWeapon.ShotOrigin,
                    ref LocalTransformLookup,
                    ref ParentLookup,
                    ref PostTransformMatrixLookup);

                TransformHelpers.ComputeWorldTransformMatrix(
                    baseWeapon.ShotOrigin,
                    out float4x4 shotVisualsOrigin,
                    ref LocalTransformLookup,
                    ref ParentLookup,
                    ref PostTransformMatrixLookup);

                baseWeapon.TotalShotsCount++;
                baseWeapon.TotalProjectilesCount++;

                float3 direction = math.forward(shotSimulationOrigin.rot);
                float3 visualPosition = shotVisualsOrigin.Translation();

                projectileBuffer.Add(new WeaponProjectileWaitToSpawnBufferData
                {
                    ID = baseWeapon.TotalProjectilesCount,
                    SimulationPosition = shotSimulationOrigin.pos,
                    SimulationDirection = direction,
                    VisualPosition = visualPosition,
                    SimulationRotation = shotSimulationOrigin.rot,
                });
            }
        }
    }
}