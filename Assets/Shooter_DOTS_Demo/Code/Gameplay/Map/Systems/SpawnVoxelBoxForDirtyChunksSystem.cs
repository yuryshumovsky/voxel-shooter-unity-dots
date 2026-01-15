using System.Diagnostics;
using Shooter_DOTS_Demo.Code.Authoring;
using Shooter_DOTS_Demo.Code.Gameplay.Map.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Shooter_DOTS_Demo.Code.Gameplay.Map.Systems
{
    [UpdateInGroup(typeof(VoxelMapSystemGroup))]
    [UpdateAfter(typeof(ClearPreviousBoxesForDirtyChunksSystem))]
    public partial struct SpawnVoxelBoxForDirtyChunksSystem : ISystem
    {
        private Random _random;
        private NativeList<MeshBoxSpawnDataBufferElement> _boxesForSpawn;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EntitiesReferencesComponent>();
            _boxesForSpawn = new NativeList<MeshBoxSpawnDataBufferElement>(256, Allocator.Persistent);
        }

        public void OnUpdate(ref SystemState state)
        {
            EntitiesReferencesComponent entitiesReferences = SystemAPI.GetSingleton<EntitiesReferencesComponent>();

            EntityCommandBuffer spawnEcb = new EntityCommandBuffer(Allocator.TempJob);

            _boxesForSpawn.Clear();

            foreach ((
                         RefRW<ChunkComponent> chunkComponent,
                         DynamicBuffer<MeshBoxSpawnDataBufferElement> spawnDataBuffer,
                         Entity chunkEntity
                     ) in SystemAPI
                         .Query<
                             RefRW<ChunkComponent>,
                             DynamicBuffer<MeshBoxSpawnDataBufferElement>
                         >()
                         .WithAll<ChunkDirtyEnableTagComponent>()
                         .WithEntityAccess()
                    )
            {
                if (!spawnDataBuffer.IsCreated)
                {
                    continue;
                }

                foreach (MeshBoxSpawnDataBufferElement boxData in spawnDataBuffer)
                {
                    _boxesForSpawn.Add(boxData);
                }

                spawnDataBuffer.Clear();

                spawnEcb.SetComponentEnabled<ChunkDirtyEnableTagComponent>(chunkEntity, false);
            }

            if (_boxesForSpawn.Length > 0)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                SpawnMeshBoxJob spawnMeshBoxJob = new SpawnMeshBoxJob
                {
                    Prototype = entitiesReferences.brickPrefabEntity,
                    Ecb = spawnEcb.AsParallelWriter(),
                    SpawnArray = _boxesForSpawn,
                    FrameCount = (uint)Time.frameCount
                };

                JobHandle spawnHandle = spawnMeshBoxJob.Schedule(
                    _boxesForSpawn.Length,
                    (int)math.pow(8f, 3f)
                );
                spawnHandle.Complete();
            }

            spawnEcb.Playback(state.EntityManager);
            spawnEcb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            if (_boxesForSpawn.IsCreated)
            {
                _boxesForSpawn.Dispose();
            }
        }
    }

    [BurstCompile]
    public struct SpawnMeshBoxJob : IJobParallelFor
    {
        [ReadOnly] public Entity Prototype;
        public EntityCommandBuffer.ParallelWriter Ecb;

        [ReadOnly] public NativeList<MeshBoxSpawnDataBufferElement> SpawnArray;
        [ReadOnly] public uint FrameCount;

        public void Execute(int index)
        {
            MeshBoxSpawnDataBufferElement meshSpawnDataBufferElement = SpawnArray[index];
            if (meshSpawnDataBufferElement.VoxelType == 0)
            {
                return;
            }

            Entity newBoxVoxelEntity = Ecb.Instantiate(index, Prototype);

            Ecb.AddComponent(index, newBoxVoxelEntity, new RequestDestroyComponent());
            Ecb.SetComponentEnabled<RequestDestroyComponent>(index, newBoxVoxelEntity, false);

            float3 shiftAfterScale = (float3)(meshSpawnDataBufferElement.Size - new int3(1, 1, 1)) / 2f;
            float3 position = meshSpawnDataBufferElement.PositionInWorld + shiftAfterScale;

            Ecb.SetComponent(index, newBoxVoxelEntity, new LocalTransform
            {
                Position = position,
                Rotation = Quaternion.identity,
                Scale = 1
            });

            float3 scale = new float3(
                meshSpawnDataBufferElement.Size.x,
                meshSpawnDataBufferElement.Size.y,
                meshSpawnDataBufferElement.Size.z);

            Ecb.AddComponent(
                index,
                newBoxVoxelEntity,
                new PostTransformMatrix
                {
                    Value = float4x4.Scale(scale)
                });

            Ecb.SetName(
                index,
                newBoxVoxelEntity,
                (FixedString64Bytes)$"draw box: {meshSpawnDataBufferElement.PositionInChunk} {newBoxVoxelEntity.Index} {FrameCount}"
            );

            Ecb.AppendToBuffer(
                index,
                meshSpawnDataBufferElement.ChunkEntity,
                new VoxelBoxEntityBufferElement
                {
                    VoxelBoxEntity = newBoxVoxelEntity
                }
            );

            Ecb.SetComponent(
                index,
                newBoxVoxelEntity,
                new URPMaterialPropertyBaseColor() { Value = meshSpawnDataBufferElement.Color }
            );

            Ecb.AddComponent(index, newBoxVoxelEntity, new VoxelBoxTagComponent());
            Ecb.AddComponent(index, newBoxVoxelEntity, new UpdateBoxColliderSizeViaScaleRequestComponent());
            Ecb.SetComponentEnabled<UpdateBoxColliderSizeViaScaleRequestComponent>(index, newBoxVoxelEntity, true);
        }
    }
}