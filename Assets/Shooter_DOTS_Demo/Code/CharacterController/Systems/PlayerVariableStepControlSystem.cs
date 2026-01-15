using Shooter_DOTS_Demo.Code.CharacterController.Components;
using Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components;
using Unity.Burst;
using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.CharacterController.Systems
{
    /// <summary>
    /// Apply inputs that need to be read at a variable rate
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct PlayerVariableStepControlSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate(SystemAPI.QueryBuilder()
                .WithAll<PlayerComponent, PlayerInputsComponent>().Build());
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (playerInputComponent, playerComponent)in SystemAPI
                         .Query<RefRO<PlayerInputsComponent>, RefRO<PlayerComponent>>()
                         .WithAll<Simulate>()
                    )
            {
                Entity characterEntity = playerComponent.ValueRO.ControlledCharacter;

                if (SystemAPI.HasComponent<CharacterControlComponent>(characterEntity))
                {
                    CharacterControlComponent characterControlComponent =
                        SystemAPI.GetComponent<CharacterControlComponent>(characterEntity);

                    characterControlComponent.LookDegreesDelta = playerInputComponent.ValueRO.LookInput;

                    SystemAPI.SetComponent(characterEntity, characterControlComponent);
                }

                if (SystemAPI.HasComponent<ActiveWeaponComponent>(characterEntity))
                {
                    ActiveWeaponComponent activeWeapon = SystemAPI.GetComponent<ActiveWeaponComponent>(characterEntity);
                    Entity weaponEntity = activeWeapon.CurrentWeaponEntity;

                    if (weaponEntity != Entity.Null && SystemAPI.HasComponent<WeaponControlComponent>(weaponEntity))
                    {
                        WeaponControlComponent weaponControl =
                            SystemAPI.GetComponent<WeaponControlComponent>(weaponEntity);
                        weaponControl.ShootPressed = playerInputComponent.ValueRO.ShootPressed;
                        weaponControl.ShootReleased = playerInputComponent.ValueRO.ShootReleased;
                        weaponControl.AimHeld = playerInputComponent.ValueRO.AimHeld;

                        SystemAPI.SetComponent(weaponEntity, weaponControl);
                    }
                }
            }
        }
    }
}