using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using RaycastHit = Unity.Physics.RaycastHit;
using Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components;
using Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components.Projectile;
using Shooter_DOTS_Demo.Code.Gameplay.Weapons.Utilities;
using Shooter_DOTS_Demo.Code.Misc;
using Shooter_DOTS_Demo.Code.Authoring;
using Shooter_DOTS_Demo.Code.Gameplay.Map.Components;
using Unity.Collections.LowLevel.Unsafe;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(ProjectileMovementSystem))]
    public partial struct ProjectileHitDetectSystem : ISystem
    {
        private NativeList<RaycastHit> _hits;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.RequireForUpdate<EntitiesReferencesComponent>();
            _hits = new NativeList<RaycastHit>(128, Allocator.Persistent);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer.ParallelWriter ecb = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

            ProjectileHitJob projectileHitJob = new ProjectileHitJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                PhysicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld,
                ECB = ecb,
                Hits = _hits
            };
            state.Dependency = projectileHitJob.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            if (_hits.IsCreated)
            {
                _hits.Dispose();
            }
        }

        [BurstCompile]
        [WithAll(typeof(Simulate))]
        public partial struct ProjectileHitJob : IJobEntity, IJobEntityChunkBeginEnd
        {
            public float DeltaTime;
            [ReadOnly] public PhysicsWorld PhysicsWorld;
            public EntityCommandBuffer.ParallelWriter ECB;

            [NativeDisableContainerSafetyRestriction]
            public NativeList<RaycastHit> Hits;

            void Execute(
                Entity projectileEntity,
                [EntityIndexInQuery] int entityInQueryIndex,
                ref ProjectileComponent projectileComponent,
                ref LocalTransform localTransform,
                in DynamicBuffer<WeaponShotIgnoredEntityBufferData> ignoredEntities)
            {
                if (projectileComponent.HasHit != 0)
                {
                    return;
                }

                float3 velocityWithGravity =
                    projectileComponent.Velocity + (math.up() * projectileComponent.Gravity) * DeltaTime;
                float3 displacement = velocityWithGravity * DeltaTime;

                Hits.Clear();
                RaycastInput raycastInput = new RaycastInput
                {
                    Start = localTransform.Position,
                    End = localTransform.Position + displacement,
                    Filter = CollisionFilter.Default,
                };
                PhysicsWorld.CastRay(raycastInput, ref Hits);

                if (WeaponUtilities.GetClosestValidWeaponRaycastHit(
                        in Hits,
                        in ignoredEntities,
                        out RaycastHit closestValidHit))
                {
                    float3 hitPosition = localTransform.Position + displacement * closestValidHit.Fraction;
                    float3 rayDirection = math.normalizesafe(displacement);
                    hitPosition += rayDirection * 0.001f;
                    projectileComponent.HitEntity = closestValidHit.Entity;
                    projectileComponent.HasHit = 1;
                    localTransform.Position = hitPosition;

                    ECB.SetComponentEnabled<DelayedDespawnComponent>(entityInQueryIndex, projectileEntity, true);
                    
                    Entity requestDestroyMapVoxelsEntity = ECB.CreateEntity(entityInQueryIndex);
                    ECB.AddComponent(
                        entityInQueryIndex,
                        requestDestroyMapVoxelsEntity,
                        new ExplosionVoxelsAtPositionRequestComponent()
                        {
                            HitWorldPosition = hitPosition,
                            ExplosionRadius = 3
                        });
                }
            }


            public bool OnChunkBegin(
                in ArchetypeChunk chunk,
                int unfilteredChunkIndex,
                bool useEnabledMask,
                in v128 chunkEnabledMask)
            {
                Hits.Clear();
                return true;
            }

            public void OnChunkEnd(
                in ArchetypeChunk chunk, 
                int unfilteredChunkIndex, 
                bool useEnabledMask,
                in v128 chunkEnabledMask,
                bool chunkWasExecuted)
            {
            }
        }
    }
}