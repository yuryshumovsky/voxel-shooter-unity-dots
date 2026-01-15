using System.Collections.Generic;
using System.Diagnostics;
using Shooter_DOTS_Demo.Code.Gameplay.Map.Components;
using Shooter_DOTS_Demo.Code.Gameplay.Map.Components.Singletons;
using Shooter_DOTS_Demo.Code.Gameplay.Map.Components.Voxels;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Shooter_DOTS_Demo.Code.Gameplay.Map.Services
{
    public class MapVoxelsAndChunksInitializeService
    {
        public void InitializeChunksFromVoxels(Dictionary<int3, byte> loadedVoxels, int chunkSize)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            World world = World.DefaultGameObjectInjectionWorld;
            EntityManager entityManager = world.EntityManager;

            ChunksMapSingletonComponent chunksMapSingletonComponent = new ChunksMapSingletonComponent
            {
                ChunksMap = new NativeHashMap<int3, Entity>(1024, Allocator.Persistent)
            };

            entityManager.CreateSingleton(chunksMapSingletonComponent);
            
            int3 max = int3.zero;

            foreach (int3 key in loadedVoxels.Keys)
            {
                max.x = math.max(max.x, key.x);
                max.y = math.max(max.y, key.y);
                max.z = math.max(max.z, key.z);
            }

            int chunkCapacity = (int)math.pow(chunkSize, 3);

            int chunksXCount = (int)math.ceil((float)(max.x + 1) / (float)chunkSize);
            int chunksYCount = (int)math.ceil((float)(max.y + 1) / (float)chunkSize);
            int chunksZCount = (int)math.ceil((float)(max.z + 1) / (float)chunkSize);
            
            EntityArchetype voxelEntityArchetype = entityManager.CreateArchetype(
                typeof(VoxelTypeComponent),
                typeof(PositionComponent),
                typeof(FloodFillStartPointTagComponent),
                typeof(GroundedVoxelTagComponent),
                typeof(WaitConvertToFallVoxelComponent)
            );

            int chunksCount = chunksXCount * chunksYCount * chunksZCount;
            int voxelsCount = chunksCount * (int)math.pow(chunkSize, 3);
            
            VoxelMapSingletonComponent voxelsSingletonComponent = new VoxelMapSingletonComponent
            {
                VoxelsMap = new NativeParallelHashMap<int3, Entity>(voxelsCount, Allocator.Persistent)
            };
            
            
            NativeArray<Entity> voxelsEntities = entityManager.CreateEntity(
                voxelEntityArchetype,
                voxelsCount,
                Allocator.Temp
            );

            int voxelEntityIndex = 0;

            for (int chunkX = 0; chunkX < chunksXCount; chunkX++)
            {
                for (int chunkY = 0; chunkY < chunksYCount; chunkY++)
                {
                    for (int chunkZ = 0; chunkZ < chunksZCount; chunkZ++)
                    {
                        Entity chunkEntity = entityManager.CreateEntity();
                        entityManager.SetName(chunkEntity, (FixedString64Bytes)$"chunk_{chunkX}_{chunkY}_{chunkZ}");

                        int3 chunkPosition = new int3(chunkX * chunkSize, chunkY * chunkSize, chunkZ * chunkSize);
                        entityManager.AddComponentData(chunkEntity, new ChunkComponent
                        {
                            ChunkPosition = chunkPosition,
                            ChunkSize = chunkSize,
                        });

                        entityManager.AddComponent<ChunkDirtyEnableTagComponent>(chunkEntity);
                        entityManager.SetComponentEnabled<ChunkDirtyEnableTagComponent>(chunkEntity, true);

                        chunksMapSingletonComponent.ChunksMap.Add(new int3(chunkX, chunkY, chunkZ), chunkEntity);

                        entityManager.AddBuffer<MeshBoxSpawnDataBufferElement>(chunkEntity);
                        entityManager.AddBuffer<VoxelBoxEntityBufferElement>(chunkEntity);
                        entityManager.AddBuffer<VoxelEntityBufferElement>(chunkEntity);

                        DynamicBuffer<VoxelEntityBufferElement> chunkVoxelEntitiesBuffer = entityManager
                            .GetBuffer<VoxelEntityBufferElement>(chunkEntity);

                        chunkVoxelEntitiesBuffer.Capacity = chunkCapacity;

                        for (int voxelZ = 0; voxelZ < chunkSize; voxelZ++)
                        for (int voxelY = 0; voxelY < chunkSize; voxelY++)
                        for (int voxelX = 0; voxelX < chunkSize; voxelX++)
                        {
                            int3 voxelPosition = new int3(
                                voxelX + chunkPosition.x,
                                voxelY + chunkPosition.y,
                                voxelZ + chunkPosition.z
                            );

                            byte voxelType = 0;

                            loadedVoxels.TryGetValue(voxelPosition, out voxelType);

                            Entity voxelEntity = voxelsEntities[voxelEntityIndex++];
                            entityManager.SetComponentData(voxelEntity, new VoxelTypeComponent { Value = voxelType });
                            entityManager.SetComponentData(voxelEntity, new PositionComponent { Value = voxelPosition });
                            
                            entityManager.SetComponentEnabled<FloodFillStartPointTagComponent>(voxelEntity, false);
                            entityManager.SetComponentEnabled<GroundedVoxelTagComponent>(voxelEntity, false);
                            entityManager.SetComponentEnabled<WaitConvertToFallVoxelComponent>(voxelEntity, false);

                            voxelsSingletonComponent.VoxelsMap.TryAdd(voxelPosition, voxelEntity);

                            chunkVoxelEntitiesBuffer.Add(new VoxelEntityBufferElement
                            {
                                VoxelEntity = voxelEntity
                            });
                        }
                    }
                }
            }

            entityManager.CreateSingleton(voxelsSingletonComponent);

            stopwatch.Stop();
        }
    }
}