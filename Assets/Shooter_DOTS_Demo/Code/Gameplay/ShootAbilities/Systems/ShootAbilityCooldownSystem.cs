using Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.Components;
using Unity.Burst;
using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.Systems
{
    public partial struct ShootAbilityCooldownSystem : ISystem
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

            foreach ((
                         RefRW<AbilityCooldownComponent> abilityCooldown,
                         Entity abilityEntity
                     ) in SystemAPI.Query<
                         RefRW<AbilityCooldownComponent>
                     >().WithEntityAccess()
                    )
            {
                abilityCooldown.ValueRW.timer += SystemAPI.Time.DeltaTime;

                if (abilityCooldown.ValueRW.timer >= abilityCooldown.ValueRW.timerMax)
                {
                    abilityCooldown.ValueRW.timer = 0;
                    abilityCooldown.ValueRW.done = true;
                    entityCommandBuffer.SetComponentEnabled<AbilityCooldownComponent>(abilityEntity, false);
                }
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}