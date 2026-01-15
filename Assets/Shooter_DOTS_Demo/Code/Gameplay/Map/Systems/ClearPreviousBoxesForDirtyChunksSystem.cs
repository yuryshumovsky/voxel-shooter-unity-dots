using Shooter_DOTS_Demo.Code.Gameplay.Map.Components;
using Unity.Burst;
using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Gameplay.Map.Systems
{
    [UpdateInGroup(typeof(VoxelMapSystemGroup))]
    [UpdateBefore(typeof(SpawnVoxelBoxForDirtyChunksSystem))]
    public partial struct ClearPreviousBoxesForDirtyChunksSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer entityCommandBuffer = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (DynamicBuffer<VoxelBoxEntityBufferElement> voxelBoxEntityBuffer in SystemAPI
                         .Query<DynamicBuffer<VoxelBoxEntityBufferElement>>()
                         .WithAll<ChunkDirtyEnableTagComponent>())
            {
                foreach (VoxelBoxEntityBufferElement element in voxelBoxEntityBuffer)
                {
                    entityCommandBuffer.DestroyEntity(element.VoxelBoxEntity);
                }

                voxelBoxEntityBuffer.Clear();
            }
        }
    }
}