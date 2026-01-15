using Shooter_DOTS_Demo.Code.Gameplay.Map.Components.Singletons;
using Shooter_DOTS_Demo.Code.Gameplay.Map.Components.Voxels;
using Shooter_DOTS_Demo.Code.Infrastructure.States.GameStates;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Shooter_DOTS_Demo.Code.Gameplay.Map.Systems
{
    [UpdateInGroup(typeof(VoxelMapSystemGroup))]
    [UpdateAfter(typeof(ProcessExplosionRequestSystem))]
    public partial struct FindUnsupportedVoxelsSystem : ISystem
    {
        private ComponentLookup<VoxelTypeComponent> voxelTypeLookup;
        private ComponentLookup<PositionComponent> positionLookup;
        private ComponentLookup<GroundedVoxelTagComponent> groundedLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<VoxelMapSingletonComponent>();
            state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<FloodFillStartPointTagComponent>().Build());

            voxelTypeLookup = SystemAPI.GetComponentLookup<VoxelTypeComponent>(false);
            positionLookup = SystemAPI.GetComponentLookup<PositionComponent>(true);
            groundedLookup = SystemAPI.GetComponentLookup<GroundedVoxelTagComponent>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingleton(out VoxelMapSingletonComponent voxelMapSingleton))
            {
                return;
            }

            voxelTypeLookup.Update(ref state);
            positionLookup.Update(ref state);
            groundedLookup.Update(ref state);

            EntityCommandBuffer ecb = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            NativeHashSet<int3> hasGroundHashSet = new NativeHashSet<int3>(1024, Allocator.Temp);
            NativeHashSet<int3> noGroundHashSet = new NativeHashSet<int3>(1024, Allocator.Temp);
            NativeHashSet<int3> processedPositionHasSet = new NativeHashSet<int3>(1024, Allocator.Temp);
            
            foreach (var (_, positionComponent, entity) in SystemAPI
                         .Query<
                             EnabledRefRO<FloodFillStartPointTagComponent>,
                             RefRO<PositionComponent>
                         >()
                         .WithEntityAccess())
            {
                NativeList<Entity> chain = new NativeList<Entity>(64, Allocator.Temp);
                NativeQueue<int3> queueForProcessing = new NativeQueue<int3>(Allocator.Temp);

                int3 startPos = positionComponent.ValueRO.Value;
                queueForProcessing.Enqueue(startPos);

                bool groundFounded = false;

                while (queueForProcessing.Count > 0)
                {
                    int3 currentPos = queueForProcessing.Dequeue();
                    Entity tempEntity = Entity.Null;

                    if (!voxelMapSingleton.VoxelsMap.TryGetValue(currentPos, out tempEntity))
                    {
                        continue;
                    }

                    if (voxelTypeLookup.GetRefRO(tempEntity).ValueRO.Value == 0)
                    {
                        continue;
                    }

                    chain.Add(tempEntity);

                    if (hasGroundHashSet.Contains(currentPos))
                    {
                        groundFounded = true;
                        break;
                    }

                    if (processedPositionHasSet.Contains(currentPos))
                    {
                        continue;
                    }
                    
                    processedPositionHasSet.Add(currentPos);
                    
                    if (groundedLookup.IsComponentEnabled(tempEntity))
                    {
                        groundFounded = true;
                        break;
                    }

                    for (int i = 0; i < GameConstants.NeighborOffsetsCount; i++)
                    {
                        int3 neighborPos = currentPos + GameConstants.NeighborOffsets[i];
                        queueForProcessing.Enqueue(neighborPos);
                    }
                }

                if (groundFounded)
                {
                    foreach (Entity chainEntity in chain)
                    {
                        int3 position = positionLookup.GetRefRO(chainEntity).ValueRO.Value;
                        hasGroundHashSet.Add(position);
                    }
                }
                else
                {
                    foreach (Entity chainEntity in chain)
                    {
                        ecb.SetComponentEnabled<WaitConvertToFallVoxelComponent>(chainEntity, true);

                        int3 position = positionLookup.GetRefRO(chainEntity).ValueRO.Value;
                        noGroundHashSet.Add(position);
                    }
                }

                chain.Dispose();
                queueForProcessing.Dispose();

                ecb.SetComponentEnabled<FloodFillStartPointTagComponent>(entity, false);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}