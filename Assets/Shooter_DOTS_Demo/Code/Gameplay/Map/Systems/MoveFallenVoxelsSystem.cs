using Shooter_DOTS_Demo.Code.Gameplay.Map.Components.Voxels;
using Shooter_DOTS_Demo.Code.Utils;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Shooter_DOTS_Demo.Code.Gameplay.Map.Systems
{
    [UpdateInGroup(typeof(VoxelMapSystemGroup))]
    [UpdateAfter(typeof(MakeFallenVoxelsSystem))]
    public partial struct MoveFallenVoxelsSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;

            ProjectileMovementJob job = new ProjectileMovementJob
            {
                DeltaTime = deltaTime,
            };
            state.Dependency = job.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        [WithAll(typeof(Simulate))]
        public partial struct ProjectileMovementJob : IJobEntity
        {
            public float DeltaTime;

            void Execute(
                ref FallenVoxelComponent fallenVoxelComponent,
                ref LocalTransform localTransform)
            {
                /*if (projectileComponent.HasHit != 0)
            {
                return;
            }*/

                fallenVoxelComponent.Velocity += (math.up() * fallenVoxelComponent.Gravity) * DeltaTime;
                float3 displacement = fallenVoxelComponent.Velocity * DeltaTime;
                localTransform.Position += displacement;

                int3 voxelPosition = PositionUtils.GetVoxelPositionFromWorldPosition(
                    localTransform.Position + new float3(0, 0.5f, 0)
                );
                fallenVoxelComponent.CurrentPosition = voxelPosition;
            }
        }
    }
}