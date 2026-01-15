using Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.Components;
using Unity.Burst;
using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.Systems
{
    public partial struct AbilityChooseProcessSystem : ISystem
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

            foreach ((RefRO<ChooseAbilityRequestComponent> chooseRequest, Entity requestEntity)
                     in SystemAPI
                         .Query<RefRO<ChooseAbilityRequestComponent>>()
                         .WithEntityAccess()
                    )
            {
                Entity chooseEntity = Entity.Null;
                foreach (
                    (
                        RefRO<AbilityCooldownComponent> cooldown,
                        RefRW<AbilityViewDataComponent> viewData,
                        Entity abilityEntity
                    )
                    in SystemAPI.Query
                    <
                        RefRO<AbilityCooldownComponent>,
                        RefRW<AbilityViewDataComponent>
                    >().WithEntityAccess()
                )
                {
                    if (cooldown.ValueRO.done)
                    {
                        chooseEntity = abilityEntity;
                    }
                }

                entityCommandBuffer.DestroyEntity(requestEntity);
            }
        }
    }
}