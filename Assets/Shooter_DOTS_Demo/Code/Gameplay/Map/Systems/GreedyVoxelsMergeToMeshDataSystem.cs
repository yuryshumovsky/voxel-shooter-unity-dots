using Shooter_DOTS_Demo.Code.Gameplay.Map.Components;
using Shooter_DOTS_Demo.Code.Gameplay.Map.Components.Voxels;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Shooter_DOTS_Demo.Code.Gameplay.Map.Systems
{
    [UpdateInGroup(typeof(VoxelMapSystemGroup))]
    [UpdateBefore(typeof(SpawnVoxelBoxForDirtyChunksSystem))]
    public partial struct GreedyVoxelsMergeToMeshDataSystem : ISystem
    {
        private NativeList<Entity> _dirtyChunks;
        private ComponentLookup<ChunkComponent> _chunkComponentLookup;
        private BufferLookup<MeshBoxSpawnDataBufferElement> _meshBoxBufferLookup;
        private BufferLookup<VoxelEntityBufferElement> _voxelEntityBufferLookup;
        private ComponentLookup<VoxelTypeComponent> _voxelTypeLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _dirtyChunks = new NativeList<Entity>(64, Allocator.Persistent);
            _chunkComponentLookup = state.GetComponentLookup<ChunkComponent>(false);
            _meshBoxBufferLookup = state.GetBufferLookup<MeshBoxSpawnDataBufferElement>(false);
            _voxelEntityBufferLookup = state.GetBufferLookup<VoxelEntityBufferElement>(true);
            _voxelTypeLookup = state.GetComponentLookup<VoxelTypeComponent>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _dirtyChunks.Clear();

            foreach (var (chunkComponent, chunkEntity) in SystemAPI
                         .Query<RefRO<ChunkComponent>>()
                         .WithAll<
                             ChunkDirtyEnableTagComponent,
                             MeshBoxSpawnDataBufferElement,
                             VoxelEntityBufferElement>()
                         .WithEntityAccess())
            {
                _dirtyChunks.Add(chunkEntity);
            }

            if (_dirtyChunks.Length == 0)
            {
                return;
            }

            _chunkComponentLookup.Update(ref state);
            _meshBoxBufferLookup.Update(ref state);
            _voxelEntityBufferLookup.Update(ref state);
            _voxelTypeLookup.Update(ref state);

            int currentFrame = Time.frameCount;
            ProcessDirtyChunkJob processDirtyJob = new ProcessDirtyChunkJob
            {
                DirtyEntities = _dirtyChunks.AsArray(),
                ChunkComponentLookup = _chunkComponentLookup,
                MeshBoxBufferLookup = _meshBoxBufferLookup,
                VoxelEntityBufferLookup = _voxelEntityBufferLookup,
                VoxelTypeLookup = _voxelTypeLookup,
                CurrentFrame = currentFrame
            };

            state.Dependency = processDirtyJob.Schedule(_dirtyChunks.Length, 8, state.Dependency);
            state.Dependency.Complete();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            if (_dirtyChunks.IsCreated)
            {
                _dirtyChunks.Dispose();
            }
        }

        [BurstCompile]
        public struct ProcessDirtyChunkJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<Entity> DirtyEntities;
            [NativeDisableParallelForRestriction] public ComponentLookup<ChunkComponent> ChunkComponentLookup;

            [NativeDisableParallelForRestriction]
            public BufferLookup<MeshBoxSpawnDataBufferElement> MeshBoxBufferLookup;

            [ReadOnly] public BufferLookup<VoxelEntityBufferElement> VoxelEntityBufferLookup;
            [ReadOnly] public ComponentLookup<VoxelTypeComponent> VoxelTypeLookup;
            [ReadOnly] public int CurrentFrame;

            public void Execute(int index)
            {
                Entity entity = DirtyEntities[index];
                
                if (!ChunkComponentLookup.TryGetComponent(entity, out var chunkComponent))
                    return;

                if (!MeshBoxBufferLookup.TryGetBuffer(entity, out var meshBoxBuffer))
                    return;

                if (!VoxelEntityBufferLookup.TryGetBuffer(entity, out var voxelEntities))
                    return;

                meshBoxBuffer.Clear();

                int size = chunkComponent.ChunkSize;

                if (voxelEntities.Length == 0 || size <= 0)
                    return;

                VolumeGreedy(in voxelEntities, size, entity, ref meshBoxBuffer, VoxelTypeLookup);
            }

            [BurstCompile]
            private static void VolumeGreedy(
                in DynamicBuffer<VoxelEntityBufferElement> voxelEntities,
                int size,
                Entity chunkEntity,
                ref DynamicBuffer<MeshBoxSpawnDataBufferElement> output,
                ComponentLookup<VoxelTypeComponent> voxelTypeLookup
            )
            {
                int total = size * size * size;
                var visited = new NativeArray<byte>(total, Allocator.Temp);

                for (int z = 0; z < size; z++)
                for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    int idx = ToIndex(x, y, z, size);
                    if (visited[idx] != 0) continue;

                    Entity voxelEntity = voxelEntities[idx].VoxelEntity;
                    if (!voxelTypeLookup.TryGetComponent(voxelEntity, out var voxelTypeComponent))
                    {
                        visited[idx] = 1;
                        continue;
                    }

                    byte type = voxelTypeComponent.Value;
                    if (type == 0)
                    {
                        visited[idx] = 1;
                        continue;
                    }

                    int maxX = x;
                    for (int xx = x + 1; xx < size; xx++)
                    {
                        int i = ToIndex(xx, y, z, size);
                        if (visited[i] != 0) break;
                        Entity ve = voxelEntities[i].VoxelEntity;
                        if (!voxelTypeLookup.TryGetComponent(ve, out var vt) || vt.Value != type) break;
                        maxX = xx;
                    }

                    int width = maxX - x + 1;

                    int maxY = y;
                    for (int yy = y + 1; yy < size; yy++)
                    {
                        bool ok = true;
                        for (int xx = x; xx <= maxX; xx++)
                        {
                            int i = ToIndex(xx, yy, z, size);
                            if (visited[i] != 0)
                            {
                                ok = false;
                                break;
                            }

                            Entity ve = voxelEntities[i].VoxelEntity;
                            if (!voxelTypeLookup.TryGetComponent(ve, out var vt) || vt.Value != type)
                            {
                                ok = false;
                                break;
                            }
                        }

                        if (!ok) break;
                        maxY = yy;
                    }

                    int height = maxY - y + 1;

                    int maxZ = z;
                    for (int zz = z + 1; zz < size; zz++)
                    {
                        bool ok = true;
                        for (int yy = y; yy <= maxY && ok; yy++)
                        {
                            for (int xx = x; xx <= maxX; xx++)
                            {
                                int i = ToIndex(xx, yy, zz, size);
                                if (visited[i] != 0)
                                {
                                    ok = false;
                                    break;
                                }

                                Entity ve = voxelEntities[i].VoxelEntity;
                                if (!voxelTypeLookup.TryGetComponent(ve, out var vt) || vt.Value != type)
                                {
                                    ok = false;
                                    break;
                                }
                            }
                        }

                        if (!ok) break;
                        maxZ = zz;
                    }

                    int depth = maxZ - z + 1;

                    for (int zz = z; zz <= maxZ; zz++)
                    for (int yy = y; yy <= maxY; yy++)
                    for (int xx = x; xx <= maxX; xx++)
                    {
                        int index = ToIndex(xx, yy, zz, size);
                        visited[index] = 1;
                    }

                    output.Add(new MeshBoxSpawnDataBufferElement
                    {
                        VoxelType = type,
                        PositionInChunk = new int3(x, y, z),
                        Size = new int3(width, height, depth),
                        ChunkEntity = chunkEntity
                    });
                }

                visited.Dispose();
            }

            [BurstCompile]
            static int ToIndex(int x, int y, int z, int size) => x + y * size + z * size * size;
        }
    }
}