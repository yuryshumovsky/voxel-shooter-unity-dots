using Shooter_DOTS_Demo.Code.Authoring;
using Shooter_DOTS_Demo.Code.Gameplay.Map.Components;
using Shooter_DOTS_Demo.Code.Gameplay.Map.Components.Singletons;
using Shooter_DOTS_Demo.Code.Gameplay.Map.Components.Voxels;
using Shooter_DOTS_Demo.Code.Infrastructure.States.GameStates;
using Shooter_DOTS_Demo.Code.Utils;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace Shooter_DOTS_Demo.Code.Gameplay.Map.Systems
{
    [UpdateInGroup(typeof(VoxelMapSystemGroup))]
    [UpdateAfter(typeof(FindUnsupportedVoxelsSystem))]
    public partial struct MakeFallenVoxelsSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ChunksMapSingletonComponent>();
            state.RequireForUpdate<EntitiesReferencesComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingleton(out ChunksMapSingletonComponent chunksMapSingleton))
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

            foreach (var (
                         voxelType,
                         _,
                         position,
                         entity) in SystemAPI
                         .Query<RefRW<VoxelTypeComponent>,
                             EnabledRefRO<WaitConvertToFallVoxelComponent>,
                             RefRO<PositionComponent>>()
                         .WithEntityAccess())
            {
                ecb.SetComponentEnabled<WaitConvertToFallVoxelComponent>(entity, false);

                byte originalVoxelType = voxelType.ValueRO.Value;
                voxelType.ValueRW.Value = 0;

                int3 voxelPosition = position.ValueRO.Value;
                int3 chunkPosition = PositionUtils.GetChunkPositionFromWorldPosition(voxelPosition);
                int3 chunkIndices = chunkPosition / GameConstants.ChunkSize;

                if (chunksMapSingleton.ChunksMap.TryGetValue(chunkIndices, out Entity chunkEntity))
                {
                    SystemAPI.SetComponentEnabled<ChunkDirtyEnableTagComponent>(chunkEntity, true);
                }

                if (entitiesReferences.colorsBlob.IsCreated && originalVoxelType > 0)
                {
                    ref ColorsBlob colors = ref entitiesReferences.colorsBlob.Value;
                    float4 color = colors.colorsArray[originalVoxelType];

                    float3 worldPosition = PositionUtils.GetWorldPositionFromVoxelPosition(voxelPosition);
                    Entity newVoxelEntity = ecb.Instantiate(entitiesReferences.brickPrefabEntity);

                    ecb.SetComponent(newVoxelEntity, new LocalTransform
                    {
                        Position = worldPosition,
                        Rotation = quaternion.identity,
                        Scale = 1f
                    });

                    ecb.SetComponent(newVoxelEntity, new URPMaterialPropertyBaseColor { Value = color });
                    ecb.AddComponent(newVoxelEntity, new FallenVoxelComponent ()
                    {
                        PreviousPositionCheck = voxelPosition,
                        Gravity = new float3(0,-3.5f,0),
                        Velocity = float3.zero
                    });
                }
            }
        }
    }
}