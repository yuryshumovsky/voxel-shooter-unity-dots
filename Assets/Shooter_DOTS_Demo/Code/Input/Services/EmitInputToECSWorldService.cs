using Shooter_DOTS_Demo.Code.Input.Components;
using Shooter_DOTS_Demo.Code.Utils;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Shooter_DOTS_Demo.Code.Input.Services
{
    public class EmitInputToECSWorldService : IInitializable
    {
        private readonly InputActionAsset _inputActionAsset;

        private Entity _inputEntity;
        private EntityManager _entityManager;
        private InputActionMap _player;
        private InputAction _playerMove;
        private InputAction _playerLook;

        public EmitInputToECSWorldService(InputActionAsset inputActionAsset)
        {
            _inputActionAsset = inputActionAsset;
        }

        public void Initialize()
        {
            _inputEntity = EntityUtils
                    .CreateEntity()
                    .AddComponent(new InputTagComponent())
                    .AddComponent(new LeftMouseDownComponent())
                    .SetComponentEnabled<LeftMouseDownComponent>(false)
                    .AddComponent(new MoveInputComponent())
                    .SetComponentEnabled<MoveInputComponent>(false)
                    .SetName("input")
                ;

            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            _player = _inputActionAsset.FindActionMap("Player", throwIfNotFound: true);
            _playerMove = _inputActionAsset.FindAction("Move", throwIfNotFound: true);
            _playerLook = _inputActionAsset.FindAction("Look", throwIfNotFound: true);

            SubscribeActions();
        }

        private void SubscribeActions()
        {
            _playerMove.performed += MovePerformedHandler;
            _playerMove.canceled += MoveCanceledHandler;
        }

        private void MoveCanceledHandler(InputAction.CallbackContext context)
        {
            Vector2 movementInput = context.ReadValue<Vector2>();

            _inputEntity
                .SetComponentData(new MoveInputComponent()
                    {
                        Value = (float2)movementInput
                    }
                );

        }

        private void MovePerformedHandler(InputAction.CallbackContext context)
        {
            Vector2 movementInput = context.ReadValue<Vector2>();

            _inputEntity
                .SetComponentData<MoveInputComponent>(new MoveInputComponent()
                    {
                        Value = (float2)movementInput
                    }
                )
                .SetComponentEnabled<MoveInputComponent>(true);
        }

        public void Enable()
        {
            _inputActionAsset.Enable();
        }

        public void Disable()
        {
            _inputActionAsset.Disable();
        }
    }
}