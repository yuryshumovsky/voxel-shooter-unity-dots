using Shooter_DOTS_Demo.Code.CharacterController.Components;
using Unity.Burst;
using Unity.CharacterController;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Shooter_DOTS_Demo.Code.CharacterController.Systems
{
    /// <summary>
    /// Apply inputs that need to be read at a fixed rate.
    /// It is necessary to handle this as part of the fixed step group, in case your framerate is lower than the fixed step rate.
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderFirst = true)]
    [BurstCompile]
    public partial struct PlayerFixedStepControlSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Common.Scripts.FixedTickSystem.Singleton>();
            state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<PlayerComponent, PlayerInputsComponent>().Build());
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            uint tick = SystemAPI.GetSingleton<Common.Scripts.FixedTickSystem.Singleton>().Tick;

            foreach (var (playerInputs, player) in SystemAPI
                         .Query<RefRW<PlayerInputsComponent>, RefRO<PlayerComponent>>()
                         .WithAll<Simulate>())
            {
                if (SystemAPI.HasComponent<CharacterControlComponent>(player.ValueRO.ControlledCharacter))
                {
                    CharacterControlComponent characterControlComponent =
                        SystemAPI.GetComponent<CharacterControlComponent>(player.ValueRO.ControlledCharacter);

                    quaternion characterRotation =
                        SystemAPI.GetComponent<LocalTransform>(player.ValueRO.ControlledCharacter).Rotation;

                    float3 characterForward = MathUtilities.GetForwardFromRotation(characterRotation);
                    float3 characterRight = MathUtilities.GetRightFromRotation(characterRotation);
                    characterControlComponent.MoveVector = (playerInputs.ValueRO.MoveInput.y * characterForward) +
                                                           (playerInputs.ValueRO.MoveInput.x * characterRight);
                    characterControlComponent.MoveVector =
                        MathUtilities.ClampToMaxLength(characterControlComponent.MoveVector, 1f);

                    characterControlComponent.Jump = playerInputs.ValueRO.JumpPressed.IsSet(tick);

                    SystemAPI.SetComponent(player.ValueRO.ControlledCharacter, characterControlComponent);
                }
            }
        }
    }
}