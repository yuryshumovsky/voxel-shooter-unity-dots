using Shooter_DOTS_Demo.Code.Authoring;
using Shooter_DOTS_Demo.Code.Gameplay.Map.Components.Singletons;
using Shooter_DOTS_Demo.Code.Gameplay.Map.Components.Voxels;
using Shooter_DOTS_Demo.Code.Misc;
using Shooter_DOTS_Demo.Code.Utils;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Shooter_DOTS_Demo.Code.Gameplay.Map.Systems
{
    [UpdateInGroup(typeof(VoxelMapSystemGroup))]
    [UpdateAfter(typeof(MoveFallenVoxelsSystem))]
    public partial struct FallenVoxelsLandingSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<VoxelMapSingletonComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingleton(out VoxelMapSingletonComponent voxelMapSingleton))
            {
                return;
            }

            if (!SystemAPI.TryGetSingleton(out EntitiesReferencesComponent entitiesReferences))
            {
                return;
            }

            EntityCommandBuffer ecb = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            ComponentLookup<VoxelTypeComponent> voxelTypeLookup =
                SystemAPI.GetComponentLookup<VoxelTypeComponent>(false);
            
            voxelTypeLookup.Update(ref state);

            foreach (var (fallenVoxelComponent, fallenEntity) in SystemAPI
                         .Query<RefRW<FallenVoxelComponent>>()
                         .WithEntityAccess())
            {
                if (!math.all(fallenVoxelComponent.ValueRO.PreviousPositionCheck == 
                             fallenVoxelComponent.ValueRO.CurrentPosition))
                {
                    int3 checkPosition = fallenVoxelComponent.ValueRO.CurrentPosition;
                    int3 positionBelow = checkPosition + new int3(0, -1, 0);

                    if (voxelMapSingleton.VoxelsMap.TryGetValue(positionBelow, out Entity voxelEntity))
                    {
                        if (voxelTypeLookup.TryGetComponent(voxelEntity, out var typeComponent))
                        {
                            if (typeComponent.Value > 0)
                            {
                                fallenVoxelComponent.ValueRW.PreviousPositionCheck = 
                                    fallenVoxelComponent.ValueRO.CurrentPosition;
                                
                                float3 worldPosition = PositionUtils.GetWorldPositionFromVoxelPosition(checkPosition);
                                Entity explosionEntity = ecb.Instantiate(entitiesReferences.BalloonPopExplosion);
                                ecb.SetComponent(explosionEntity, new LocalTransform
                                {
                                    Position = worldPosition,
                                    Rotation = quaternion.identity,
                                    Scale = 1f
                                });
                                ecb.AddComponent(explosionEntity, new DelayedDespawnComponent
                                {
                                    DespawnTicks = 100,
                                    Ticks = 0,
                                    HasHandledPreDespawn = 0
                                });
                                
                                ecb.DestroyEntity(fallenEntity);
                            }
                        }
                    }
                }
            }
        }
    }
}
