using Shooter_DOTS_Demo.Code.Gameplay.Map.Components;
using Unity.Collections;
using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Gameplay.Map.Systems
{
    [UpdateInGroup(typeof(VoxelMapSystemGroup))]
    public partial struct ProcessDestroyEntitiesRequestSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer entityCommandBuffer = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            EntityManager entityManager = state.EntityManager;

            foreach (var (request, entity) in SystemAPI
                         .Query<EnabledRefRO<RequestDestroyComponent>>()
                         .WithAll<RequestDestroyComponent>()
                         .WithEntityAccess()
                    )
            {
                if (entityManager.Exists(entity))
                {
                    NativeArray<ComponentType> componentTypes = entityManager.GetComponentTypes(entity);
                    foreach (ComponentType type in componentTypes)
                    {
                        entityCommandBuffer.RemoveComponent(entity, type);
                    }
                    
                }
                
                entityCommandBuffer.DestroyEntity(entity);
            }
        }
    }
}