using System;
using Unity.CharacterController;
using Unity.Entities;
using Unity.Mathematics;

namespace Shooter_DOTS_Demo.Code.CharacterController.Components
{
    [Serializable]
    public struct CharacterComponent : IComponentData
    {
        public float GroundMaxSpeed;
        public float GroundedMovementSharpness;
        public float AirAcceleration;
        public float AirMaxSpeed;
        public float AirDrag;
        public float JumpSpeed;
        public float3 Gravity;
        public bool PreventAirAccelerationAgainstUngroundedHits;
        public BasicStepAndSlopeHandlingParameters StepAndSlopeHandling;

        public float MinViewAngle;
        public float MaxViewAngle;

        public Entity ViewEntity;
        public float ViewPitchDegrees;
        public quaternion ViewLocalRotation;
    
        public Entity WeaponSocketEntity;
        public Entity WeaponAnimationSocketEntity;
    }
}