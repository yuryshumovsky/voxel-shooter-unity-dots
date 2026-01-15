using Shooter_DOTS_Demo.Code.Infrastructure.States.GameStates.Components.StateTag;
using Shooter_DOTS_Demo.Code.Infrastructure.States.StateInfrastructure;
using Shooter_DOTS_Demo.Code.Input.Services;
using Shooter_DOTS_Demo.Code.Utils;
using Unity.Entities;
using UnityEngine;

namespace Shooter_DOTS_Demo.Code.Infrastructure.States.GameStates
{
    public class GameLoopState : IState
    {
        private readonly EmitInputToECSWorldService _emitInputToEcsWorldService;
        private Entity _stateTagEntity;

        public GameLoopState(EmitInputToECSWorldService emitInputToEcsWorldService)
        {
            _emitInputToEcsWorldService = emitInputToEcsWorldService;
        }

        public void Exit()
        {
            _stateTagEntity.Destroy();
            _emitInputToEcsWorldService.Disable();
        }

        public void Enter()
        {
            _emitInputToEcsWorldService.Enable();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            _stateTagEntity = EntityUtils.CreateEntity();
            _stateTagEntity
                .AddComponent(new GameLoopStateTagComponent())
                .SetName("state_game_loop");
        }
    }
}