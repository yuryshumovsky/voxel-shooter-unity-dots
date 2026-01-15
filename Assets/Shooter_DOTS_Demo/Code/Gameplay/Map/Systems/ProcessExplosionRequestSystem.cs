using Shooter_DOTS_Demo.Code.Gameplay.Map.Components;
using Shooter_DOTS_Demo.Code.Gameplay.Map.Components.Singletons;
using Shooter_DOTS_Demo.Code.Gameplay.Map.Components.Voxels;
using Shooter_DOTS_Demo.Code.Infrastructure.States.GameStates;
using Shooter_DOTS_Demo.Code.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Shooter_DOTS_Demo.Code.Gameplay.Map.Systems
{
    [UpdateInGroup(typeof(VoxelMapSystemGroup), OrderFirst = true)]
    public partial struct ProcessExplosionRequestSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ChunksMapSingletonComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer entityCommandBuffer = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            BufferLookup<VoxelEntityBufferElement> voxelEntityBufferLookup =
                SystemAPI.GetBufferLookup<VoxelEntityBufferElement>(false);
            ComponentLookup<VoxelTypeComponent> voxelTypeLookup =
                SystemAPI.GetComponentLookup<VoxelTypeComponent>(false);
            ComponentLookup<FloodFillStartPointTagComponent> readyToFloodFillLookup =
                SystemAPI.GetComponentLookup<FloodFillStartPointTagComponent>(false);
            voxelEntityBufferLookup.Update(ref state);
            voxelTypeLookup.Update(ref state);
            readyToFloodFillLookup.Update(ref state);

            ChunksMapSingletonComponent chunksMapSingleton = SystemAPI.GetSingleton<ChunksMapSingletonComponent>();

            foreach (var (requestComponent, requestEntity) in SystemAPI
                         .Query<RefRO<ExplosionVoxelsAtPositionRequestComponent>>()
                         .WithEntityAccess())
            {
                float3 hitPos = requestComponent.ValueRO.HitWorldPosition;

                int3 hitVoxelPosition = PositionUtils.GetVoxelPositionFromWorldPosition(hitPos);

                int radius = requestComponent.ValueRO.ExplosionRadius;
                float radiusSquared = radius * (float)radius;

                uint baseSeed = (uint)requestEntity.Index;
                Random random = new Random((uint)Time.frameCount * baseSeed);
                int destroyHashSetCapacity = (int)math.pow(radius * 2, 3);
                NativeHashSet<int3> voxelPositionsToDestroy = new NativeHashSet<int3>(
                    destroyHashSetCapacity,
                    Allocator.Temp
                );

                for (int dx = -radius; dx <= radius; dx++)
                {
                    for (int dy = -radius; dy <= radius; dy++)
                    {
                        for (int dz = -radius; dz <= radius; dz++)
                        {
                            float distanceSquared = dx * dx + dy * dy + dz * dz;
                            if (distanceSquared > radiusSquared)
                            {
                                continue;
                            }

                            float normalizedDistance = math.sqrt(distanceSquared) / radius;
                            float skipProbability = normalizedDistance * 0.4f;
                            
                            if (random.NextFloat() < skipProbability)
                            {
                                continue;
                            }
                            

                            int3 currentVoxelPosition = hitVoxelPosition + new int3(dx, dy, dz);
                            int3 currentChunkPosition =
                                PositionUtils.GetChunkPositionFromWorldPosition(currentVoxelPosition);

                            int3 chunkIndices = currentChunkPosition / GameConstants.ChunkSize;
                            if (!chunksMapSingleton.ChunksMap.TryGetValue(chunkIndices, out Entity chunkEntity))
                            {
                                continue;
                            }

                            if (!voxelEntityBufferLookup.TryGetBuffer(
                                    chunkEntity,
                                    out DynamicBuffer<VoxelEntityBufferElement> voxelEntities
                                ))
                            {
                                continue;
                            }

                            int3 localPos = currentVoxelPosition - currentChunkPosition;

                            int index = PositionUtils.IndexFromXYZ(
                                localPos.x,
                                localPos.y,
                                localPos.z,
                                GameConstants.ChunkSize
                            );

                            if (index < 0 || index >= voxelEntities.Length)
                            {
                                continue;
                            }

                            Entity voxelEntity = voxelEntities[index].VoxelEntity;

                            if (voxelTypeLookup.TryGetComponent(voxelEntity, out var typeComponent))
                            {
                                if (typeComponent.Value > 0)
                                {
                                    voxelTypeLookup[voxelEntity] = new VoxelTypeComponent { Value = 0 };
                                    SystemAPI.SetComponentEnabled<ChunkDirtyEnableTagComponent>(chunkEntity, true);
                                    voxelPositionsToDestroy.Add(currentVoxelPosition);
                                }
                            }
                        }
                    }
                }

                foreach (int3 voxelPosition in voxelPositionsToDestroy)
                {
                    for (int i = 0; i < GameConstants.NeighborOffsetsCount; i++)
                    {
                        int3 neighborPosition = voxelPosition + GameConstants.NeighborOffsets[i];
                        if (voxelPositionsToDestroy.Contains(neighborPosition))
                        {
                            continue;
                        }

                        int3 neighborChunkPosition =
                            PositionUtils.GetChunkPositionFromWorldPosition(neighborPosition);
                        int3 neighborChunkIndices = neighborChunkPosition / GameConstants.ChunkSize;

                        if (!chunksMapSingleton.ChunksMap.TryGetValue(neighborChunkIndices,
                                out Entity neighborChunkEntity))
                        {
                            continue;
                        }

                        if (!voxelEntityBufferLookup.TryGetBuffer(
                                neighborChunkEntity,
                                out DynamicBuffer<VoxelEntityBufferElement> neighborVoxelEntities))
                        {
                            continue;
                        }

                        int3 neighborLocalPos = neighborPosition - neighborChunkPosition;
                        int neighborIndex = PositionUtils.IndexFromXYZ(
                            neighborLocalPos.x,
                            neighborLocalPos.y,
                            neighborLocalPos.z,
                            GameConstants.ChunkSize
                        );

                        if (neighborIndex < 0 || neighborIndex >= neighborVoxelEntities.Length)
                        {
                            continue;
                        }

                        Entity neighborVoxelEntity = neighborVoxelEntities[neighborIndex].VoxelEntity;
                        if (readyToFloodFillLookup.HasComponent(neighborVoxelEntity) &&
                            voxelTypeLookup.HasComponent(neighborVoxelEntity) &&
                            voxelTypeLookup[neighborVoxelEntity].Value > 0)
                        {
                            SystemAPI.SetComponentEnabled<FloodFillStartPointTagComponent>(
                                neighborVoxelEntity,
                                true
                            );
                        }
                    }
                }


                entityCommandBuffer.DestroyEntity(requestEntity);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}