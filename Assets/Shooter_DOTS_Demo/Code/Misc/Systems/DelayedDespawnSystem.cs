using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Collider = Unity.Physics.Collider;

namespace Shooter_DOTS_Demo.Code.Misc.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct DelayedDespawnSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            DelayedDespawnJob job = new DelayedDespawnJob
            {
                ECB = ecb,
                PhysicsColliderLookup = SystemAPI.GetComponentLookup<PhysicsCollider>(false),
            };
            state.Dependency = job.Schedule(state.Dependency);
        }

        [BurstCompile]
        public unsafe partial struct DelayedDespawnJob : IJobEntity
        {
            public EntityCommandBuffer ECB;
            public ComponentLookup<PhysicsCollider> PhysicsColliderLookup;

            void Execute(Entity entity, ref DelayedDespawnComponent delayedDespawn)
            {
                delayedDespawn.Ticks++;
                if (delayedDespawn.Ticks > delayedDespawn.DespawnTicks)
                {
                    ECB.DestroyEntity(entity);
                }

                if (delayedDespawn.HasHandledPreDespawn == 0)
                {
                    if (PhysicsColliderLookup.TryGetComponent(entity, out PhysicsCollider physicsCollider))
                    {
                        ref Collider collider = ref *physicsCollider.ColliderPtr;
                        collider.SetCollisionResponse(CollisionResponsePolicy.None);
                    }

                    delayedDespawn.HasHandledPreDespawn = 1;
                }
            }
        }
    }
}
