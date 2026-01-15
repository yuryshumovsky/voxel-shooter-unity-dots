using Shooter_DOTS_Demo.Code.Authoring;
using Shooter_DOTS_Demo.Code.Gameplay.Map.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Shooter_DOTS_Demo.Code.Gameplay.Map.Systems
{
    [UpdateInGroup(typeof(VoxelMapSystemGroup))]
    [UpdateAfter(typeof(GreedyVoxelsMergeToMeshDataSystem))]
    public partial struct SetColorForGreedyMeshDataSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EntitiesReferencesComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntitiesReferencesComponent referencesComponent = SystemAPI.GetSingleton<EntitiesReferencesComponent>();

            if (!referencesComponent.colorsBlob.IsCreated)
            {
                return;
            }
        
            SetColorToMeshBoxJob job = new SetColorToMeshBoxJob()
            {
                colorsBlob = referencesComponent.colorsBlob
            };

            JobHandle jobHandle = job.ScheduleParallel(state.Dependency);
            jobHandle.Complete();
        }

        [BurstCompile]
        [WithAll(typeof(ChunkDirtyEnableTagComponent))]
        public partial struct SetColorToMeshBoxJob : IJobEntity
        {
            public BlobAssetReference<ColorsBlob> colorsBlob;

            public void Execute(ref DynamicBuffer<MeshBoxSpawnDataBufferElement> meshBoxBufferData)
            {
                ref ColorsBlob colors = ref colorsBlob.Value;

                for (int i = 0; i < meshBoxBufferData.Length; i++)
                {
                    MeshBoxSpawnDataBufferElement element = meshBoxBufferData[i];
                    float4 newColor = colors.colorsArray[element.VoxelType];

                    element.Color = newColor;
                    meshBoxBufferData[i] = element;
                }
            }
        }
    }
}