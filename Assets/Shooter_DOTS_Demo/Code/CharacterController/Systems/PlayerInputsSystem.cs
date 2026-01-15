using Shooter_DOTS_Demo.Code.CharacterController.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.InputSystem;

namespace Shooter_DOTS_Demo.Code.CharacterController.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    [UpdateBefore(typeof(FixedStepSimulationSystemGroup))]
    public partial class PlayerInputsSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<Common.Scripts.FixedTickSystem.Singleton>();
            RequireForUpdate(SystemAPI.QueryBuilder().WithAll<PlayerComponent, PlayerInputsComponent>().Build());
        }

        protected override void OnUpdate()
        {
            uint tick = SystemAPI.GetSingleton<Common.Scripts.FixedTickSystem.Singleton>().Tick;

#if ENABLE_INPUT_SYSTEM
            foreach (var (playerInputs, player) in
                     SystemAPI.Query<RefRW<PlayerInputsComponent>, RefRO<PlayerComponent>>())
            {
                playerInputs.ValueRW.MoveInput = new float2
                {
                    x = (Keyboard.current.dKey.isPressed ? 1f : 0f) + (Keyboard.current.aKey.isPressed ? -1f : 0f),
                    y = (Keyboard.current.wKey.isPressed ? 1f : 0f) + (Keyboard.current.sKey.isPressed ? -1f : 0f),
                };

                playerInputs.ValueRW.LookInput = Mouse.current.delta.ReadValue() * player.ValueRO.LookInputSensitivity;

                if (Keyboard.current.spaceKey.wasPressedThisFrame)
                {
                    playerInputs.ValueRW.JumpPressed.Set(tick);
                }

                playerInputs.ValueRW.ShootPressed = Mouse.current.leftButton.wasPressedThisFrame;
                playerInputs.ValueRW.ShootReleased = Mouse.current.leftButton.wasReleasedThisFrame;
            }
#endif
        }
    }
}