using Shooter_DOTS_Demo.Code.CharacterController.Components;
using Shooter_DOTS_Demo.Code.Gameplay.Players.Components;
using Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components;
using Unity.CharacterController;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Shooter_DOTS_Demo.Code.CharacterController.Authoring
{
    [DisallowMultipleComponent]
    public class CharacterAuthoring : MonoBehaviour
    {
        public GameObject ViewEntity;
        public AuthoringKinematicCharacterProperties CharacterProperties = AuthoringKinematicCharacterProperties.GetDefault();

        public float GroundMaxSpeed = 10f;
        public float GroundedMovementSharpness = 15f;
        public float AirAcceleration = 50f;
        public float AirMaxSpeed = 10f;
        public float AirDrag = 0f;
        public float JumpSpeed = 10f;
        public float3 Gravity = math.up() * -30f;
        public bool PreventAirAccelerationAgainstUngroundedHits = true;
        public BasicStepAndSlopeHandlingParameters StepAndSlopeHandling = BasicStepAndSlopeHandlingParameters.GetDefault();
        public float MinViewAngle = -90f;
        public float MaxViewAngle = 90f;
    
        public GameObject WeaponSocket;
        public GameObject WeaponAnimationSocket;
    
        public class Baker : Baker<CharacterAuthoring>
        {
        
            public override void Bake(CharacterAuthoring authoring)
            {
                KinematicCharacterUtilities.BakeCharacter(this, authoring.gameObject, authoring.CharacterProperties);

                Entity entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.WorldSpace);

                AddComponent(entity, new CharacterComponent
                {
                    GroundMaxSpeed = authoring.GroundMaxSpeed,
                    GroundedMovementSharpness = authoring.GroundedMovementSharpness,
                    AirAcceleration = authoring.AirAcceleration,
                    AirMaxSpeed = authoring.AirMaxSpeed,
                    AirDrag = authoring.AirDrag,
                    JumpSpeed = authoring.JumpSpeed,
                    Gravity = authoring.Gravity,
                    PreventAirAccelerationAgainstUngroundedHits = authoring.PreventAirAccelerationAgainstUngroundedHits,
                    StepAndSlopeHandling = authoring.StepAndSlopeHandling,
                    MinViewAngle = authoring.MinViewAngle,
                    MaxViewAngle = authoring.MaxViewAngle,

                    ViewEntity = GetEntity(authoring.ViewEntity, TransformUsageFlags.Dynamic),
                    ViewPitchDegrees = 0f,
                    ViewLocalRotation = quaternion.identity,
                
                    WeaponSocketEntity = GetEntity(authoring.WeaponSocket, TransformUsageFlags.Dynamic),
                    WeaponAnimationSocketEntity = GetEntity(authoring.WeaponAnimationSocket, TransformUsageFlags.Dynamic)
                });
            
                AddComponent(entity, new CharacterControlComponent());
                AddComponent(entity, new OwningPlayerComponent());
                AddComponent(entity, new ActiveWeaponComponent());
                AddComponent(entity, new CharacterWeaponVisualFeedbackComponent());
            
                AddComponent(entity, new CharacterInitializedComponent());
                SetComponentEnabled<CharacterInitializedComponent>(entity, false);
            }
        }
    }
}
