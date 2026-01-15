using Shooter_DOTS_Demo.Code.Gameplay.Map.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Shooter_DOTS_Demo.Code.Gameplay.Map.Systems
{
    [UpdateInGroup(typeof(VoxelMapSystemGroup))]
    [UpdateAfter(typeof(SetColorForGreedyMeshDataSystem))]
    [UpdateBefore(typeof(SpawnVoxelBoxForDirtyChunksSystem))]
    public partial struct SetMeshBoxWorldPositionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            UpdateMeshBoxPositionJob job = new UpdateMeshBoxPositionJob();
            JobHandle jobHandle = job.ScheduleParallel(state.Dependency);
            jobHandle.Complete();
        }

        [BurstCompile]
        [WithAll(typeof(ChunkDirtyEnableTagComponent))]
        public partial struct UpdateMeshBoxPositionJob : IJobEntity
        {
            public void Execute(
                ref ChunkComponent chunkComponent,
                ref DynamicBuffer<MeshBoxSpawnDataBufferElement> meshBoxBuffer)
            {
                int3 chunkPosition = chunkComponent.ChunkPosition;

                for (int i = 0; i < meshBoxBuffer.Length; i++)
                {
                    MeshBoxSpawnDataBufferElement element = meshBoxBuffer[i];

                    element.PositionInWorld = element.PositionInChunk + chunkPosition;
                    meshBoxBuffer[i] = element;
                }
            }
        }
    }
}