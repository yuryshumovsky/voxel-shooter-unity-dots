using Shooter_DOTS_Demo.Code.Gameplay.Map.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Shooter_DOTS_Demo.Code.Gameplay.Map.Systems
{
    [UpdateInGroup(typeof(VoxelMapSystemGroup))]
    [UpdateAfter(typeof(SpawnVoxelBoxForDirtyChunksSystem))]
    public partial struct UpdateVoxelBoxScaleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer entityCommandBuffer = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            UpdateVoxelBoxScaleJob job = new UpdateVoxelBoxScaleJob
            {
                EntityCommandBuffer = entityCommandBuffer.AsParallelWriter()
            };

            state.Dependency = job.ScheduleParallel(state.Dependency);
        }
    }

    [BurstCompile]
    public partial struct UpdateVoxelBoxScaleJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter EntityCommandBuffer;

        private void Execute(
            Entity entity,
            [EntityIndexInQuery] int entityInQueryIndex,
            in VoxelBoxTagComponent _,
            in PostTransformMatrix postTransformMatrix,
            in PhysicsCollider physicsCollider,
            EnabledRefRW<UpdateBoxColliderSizeViaScaleRequestComponent> updateRequest)
        {
            if (!updateRequest.ValueRO)
            {
                return;
            }

            float scaleX = math.length(postTransformMatrix.Value.c0.xyz);
            float scaleY = math.length(postTransformMatrix.Value.c1.xyz);
            float scaleZ = math.length(postTransformMatrix.Value.c2.xyz);

            float3 scale = new float3(scaleX, scaleY, scaleZ);

            PhysicsCollider originalCollider = physicsCollider;

            if (originalCollider.Value.Value.Type == ColliderType.Box)
            {
                unsafe
                {
                    BoxCollider* boxColliderPtr = (BoxCollider*)originalCollider.Value.GetUnsafePtr();
                    BoxGeometry originalGeometry = boxColliderPtr->Geometry;

                    BoxGeometry scaledGeometry = new BoxGeometry
                    {
                        Center = originalGeometry.Center,
                        Size = scale,
                        Orientation = originalGeometry.Orientation,
                        BevelRadius = originalGeometry.BevelRadius
                    };

                    BlobAssetReference<Collider> scaledCollider = BoxCollider.Create(scaledGeometry);

                    EntityCommandBuffer.SetComponent(
                        entityInQueryIndex,
                        entity,
                        new PhysicsCollider
                        {
                            Value = scaledCollider
                        });
                }
            }

            updateRequest.ValueRW = false;
        }
    }
}