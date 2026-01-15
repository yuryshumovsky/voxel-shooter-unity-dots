using Shooter_DOTS_Demo.Code.Gameplay.Map.Components;
using Shooter_DOTS_Demo.Code.Gameplay.Map.Components.Voxels;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

namespace Shooter_DOTS_Demo.Code.Gameplay.Map.Systems
{
    [UpdateInGroup(typeof(VoxelMapSystemGroup), OrderFirst = true)]
    public partial struct InitializeGroundedVoxelsSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InitializeGroundedVoxelsRequestComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer entityCommandBuffer = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            InitializeGroundedVoxelsJob job = new InitializeGroundedVoxelsJob
            {
                EntityCommandBuffer = entityCommandBuffer.AsParallelWriter()
            };

            JobHandle jobHandle = job.ScheduleParallel(state.Dependency);
            jobHandle.Complete();

            foreach (var (_, requestEntity) in SystemAPI
                         .Query<RefRO<InitializeGroundedVoxelsRequestComponent>>()
                         .WithEntityAccess())
            {
                entityCommandBuffer.DestroyEntity(requestEntity);
            }
        }
    }

    [BurstCompile]
    public partial struct InitializeGroundedVoxelsJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter EntityCommandBuffer;

        private void Execute(
            Entity entity,
            [EntityIndexInQuery] int entityInQueryIndex,
            ref VoxelTypeComponent voxelTypeComponent,
            in PositionComponent positionComponent)
        {
            if (voxelTypeComponent.Value > 0 && positionComponent.Value.y == 0)
            {
                voxelTypeComponent.Value = 5;
                EntityCommandBuffer.SetComponentEnabled<GroundedVoxelTagComponent>(entityInQueryIndex, entity, true);
            }
        }
    }
}