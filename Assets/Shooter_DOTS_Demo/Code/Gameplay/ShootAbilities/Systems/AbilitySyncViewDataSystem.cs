using Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.Components;
using Shooter_DOTS_Demo.Code.Infrastructure.States.GameStates.Components.StateTag;
using Unity.Burst;
using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.Systems
{
    public partial struct AbilitySyncViewDataSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameLoopStateTagComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach ((
                         RefRO<AbilityCooldownComponent> cooldown,
                         RefRW<AbilityViewDataComponent> viewData,
                         Entity entity
                     )
                     in SystemAPI.Query<
                             RefRO<AbilityCooldownComponent>,
                             RefRW<AbilityViewDataComponent>
                         >()
                         .WithOptions(EntityQueryOptions.IncludeDisabledEntities)
                         .WithEntityAccess()
                    )
            {
                viewData.ValueRW.enableToUse = cooldown.ValueRO.done;
                viewData.ValueRW.progress = cooldown.ValueRO.timer / cooldown.ValueRO.timerMax;
            }
        }
    }
}