using Shooter_DOTS_Demo.Code.Gameplay.Map.Components;
using Shooter_DOTS_Demo.Code.Infrastructure.States.GameStates.Components;
using Shooter_DOTS_Demo.Code.Infrastructure.States.GameStates.Components.StateTag;
using Unity.Burst;
using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Gameplay.Map.Systems
{
    [UpdateInGroup(typeof(VoxelMapSystemGroup), OrderLast = true)]
    public partial struct MarkMapGenerateDoneSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GamePrepareStateTagComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            int chunkCount = 0;
            foreach (var _ in SystemAPI.Query<RefRO<ChunkComponent>>())
            {
                chunkCount++;
            }

            if (chunkCount > 0)
            {
                foreach ((RefRW<MapBuildDoneTagComponent> _, Entity entity) in
                         SystemAPI
                             .Query<RefRW<MapBuildDoneTagComponent>>()
                             .WithPresent<MapBuildDoneTagComponent>()
                             .WithEntityAccess())
                {
                    state.EntityManager.SetComponentEnabled<MapBuildDoneTagComponent>(entity, true);
                    break;
                }
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}