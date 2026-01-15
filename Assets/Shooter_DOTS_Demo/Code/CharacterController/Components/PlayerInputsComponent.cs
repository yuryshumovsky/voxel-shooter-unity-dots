using System;
using Shooter_DOTS_Demo.Code.CharacterController.Common.Scripts;
using Unity.Entities;
using Unity.Mathematics;

namespace Shooter_DOTS_Demo.Code.CharacterController.Components
{
    [Serializable]
    public struct PlayerInputsComponent : IComponentData
    {
        public float2 MoveInput;
        public float2 LookInput;
        public FixedInputEvent JumpPressed;
    
        public bool ShootPressed;
        public bool ShootReleased;
        public bool AimHeld;
    }
}