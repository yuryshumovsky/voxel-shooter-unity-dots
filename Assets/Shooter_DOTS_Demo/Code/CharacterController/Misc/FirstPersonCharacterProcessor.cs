using Shooter_DOTS_Demo.Code.CharacterController.Components;
using Unity.CharacterController;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace Shooter_DOTS_Demo.Code.CharacterController.Misc
{
    public struct FirstPersonCharacterUpdateContext
    {
        public void OnSystemCreate(ref SystemState state)
        {
        }

        public void OnSystemUpdate(ref SystemState state)
        {
        }
    }

    public struct FirstPersonCharacterProcessor : IKinematicCharacterProcessor<FirstPersonCharacterUpdateContext>
    {
        public KinematicCharacterDataAccess CharacterDataAccess;
        public RefRW<CharacterComponent> CharacterComponent;
        public RefRW<CharacterControlComponent> CharacterControl;

        public void PhysicsUpdate(ref FirstPersonCharacterUpdateContext context, ref KinematicCharacterUpdateContext baseContext)
        {
            ref CharacterComponent characterComponent = ref CharacterComponent.ValueRW;
            ref KinematicCharacterBody characterBody = ref CharacterDataAccess.CharacterBody.ValueRW;
            ref float3 characterPosition = ref CharacterDataAccess.LocalTransform.ValueRW.Position;

            KinematicCharacterUtilities.Update_Initialize(
                in this,
                ref context,
                ref baseContext,
                ref characterBody,
                CharacterDataAccess.CharacterHitsBuffer,
                CharacterDataAccess.DeferredImpulsesBuffer,
                CharacterDataAccess.VelocityProjectionHits,
                baseContext.Time.DeltaTime);

            KinematicCharacterUtilities.Update_ParentMovement(
                in this,
                ref context,
                ref baseContext,
                CharacterDataAccess.CharacterEntity,
                ref characterBody,
                CharacterDataAccess.CharacterProperties.ValueRO,
                CharacterDataAccess.PhysicsCollider.ValueRO,
                CharacterDataAccess.LocalTransform.ValueRO,
                ref characterPosition,
                characterBody.WasGroundedBeforeCharacterUpdate);

            KinematicCharacterUtilities.Update_Grounding(
                in this,
                ref context,
                ref baseContext,
                ref characterBody,
                CharacterDataAccess.CharacterEntity,
                CharacterDataAccess.CharacterProperties.ValueRO,
                CharacterDataAccess.PhysicsCollider.ValueRO,
                CharacterDataAccess.LocalTransform.ValueRO,
                CharacterDataAccess.VelocityProjectionHits,
                CharacterDataAccess.CharacterHitsBuffer,
                ref characterPosition);

            HandleVelocityControl(ref context, ref baseContext);

            KinematicCharacterUtilities.Update_PreventGroundingFromFutureSlopeChange(
                in this,
                ref context,
                ref baseContext,
                CharacterDataAccess.CharacterEntity,
                ref characterBody,
                CharacterDataAccess.CharacterProperties.ValueRO,
                CharacterDataAccess.PhysicsCollider.ValueRO,
                in characterComponent.StepAndSlopeHandling);

            KinematicCharacterUtilities.Update_GroundPushing(
                in this,
                ref context,
                ref baseContext,
                ref characterBody,
                CharacterDataAccess.CharacterProperties.ValueRO,
                CharacterDataAccess.LocalTransform.ValueRO,
                CharacterDataAccess.DeferredImpulsesBuffer,
                characterComponent.Gravity);

            KinematicCharacterUtilities.Update_MovementAndDecollisions(
                in this,
                ref context,
                ref baseContext,
                CharacterDataAccess.CharacterEntity,
                ref characterBody,
                CharacterDataAccess.CharacterProperties.ValueRO,
                CharacterDataAccess.PhysicsCollider.ValueRO,
                CharacterDataAccess.LocalTransform.ValueRO,
                CharacterDataAccess.VelocityProjectionHits,
                CharacterDataAccess.CharacterHitsBuffer,
                CharacterDataAccess.DeferredImpulsesBuffer,
                ref characterPosition);

            KinematicCharacterUtilities.Update_MovingPlatformDetection(
                ref baseContext,
                ref characterBody);

            KinematicCharacterUtilities.Update_ParentMomentum(
                ref baseContext,
                ref characterBody,
                CharacterDataAccess.LocalTransform.ValueRO.Position);

            KinematicCharacterUtilities.Update_ProcessStatefulCharacterHits(
                CharacterDataAccess.CharacterHitsBuffer,
                CharacterDataAccess.StatefulHitsBuffer);
        }

        void HandleVelocityControl(ref FirstPersonCharacterUpdateContext context, ref KinematicCharacterUpdateContext baseContext)
        {
            float deltaTime = baseContext.Time.DeltaTime;
            ref KinematicCharacterBody characterBody = ref CharacterDataAccess.CharacterBody.ValueRW;
            ref CharacterComponent characterComponent = ref CharacterComponent.ValueRW;
            ref CharacterControlComponent characterControlComponent = ref CharacterControl.ValueRW;

            if (characterBody.ParentEntity != Entity.Null)
            {
                characterControlComponent.MoveVector = math.rotate(characterBody.RotationFromParent, characterControlComponent.MoveVector);
                characterBody.RelativeVelocity = math.rotate(characterBody.RotationFromParent, characterBody.RelativeVelocity);
            }

            if (characterBody.IsGrounded)
            {
                float3 targetVelocity = characterControlComponent.MoveVector * characterComponent.GroundMaxSpeed;
                CharacterControlUtilities.StandardGroundMove_Interpolated(ref characterBody.RelativeVelocity, targetVelocity, characterComponent.GroundedMovementSharpness, deltaTime, characterBody.GroundingUp, characterBody.GroundHit.Normal);

                if (characterControlComponent.Jump)
                {
                    CharacterControlUtilities.StandardJump(ref characterBody, characterBody.GroundingUp * characterComponent.JumpSpeed, true, characterBody.GroundingUp);
                }
            }
            else
            {
                float3 airAcceleration = characterControlComponent.MoveVector * characterComponent.AirAcceleration;
                if (math.lengthsq(airAcceleration) > 0f)
                {
                    float3 tmpVelocity = characterBody.RelativeVelocity;
                    CharacterControlUtilities.StandardAirMove(ref characterBody.RelativeVelocity, airAcceleration, characterComponent.AirMaxSpeed, characterBody.GroundingUp, deltaTime, false);

                    if (characterComponent.PreventAirAccelerationAgainstUngroundedHits
                        && KinematicCharacterUtilities.MovementWouldHitNonGroundedObstruction(
                            in this,
                            ref context,
                            ref baseContext,
                            CharacterDataAccess.CharacterProperties.ValueRO,
                            CharacterDataAccess.LocalTransform.ValueRO,
                            CharacterDataAccess.CharacterEntity,
                            CharacterDataAccess.PhysicsCollider.ValueRO,
                            characterBody.RelativeVelocity * deltaTime,
                            out ColliderCastHit hit))
                    {
                        characterBody.RelativeVelocity = tmpVelocity;
                    }
                }

                CharacterControlUtilities.AccelerateVelocity(ref characterBody.RelativeVelocity, characterComponent.Gravity, deltaTime);

                CharacterControlUtilities.ApplyDragToVelocity(ref characterBody.RelativeVelocity, deltaTime, characterComponent.AirDrag);
            }
        }

        public void VariableUpdate(ref FirstPersonCharacterUpdateContext context, ref KinematicCharacterUpdateContext baseContext)
        {
            ref KinematicCharacterBody characterBody = ref CharacterDataAccess.CharacterBody.ValueRW;
            ref CharacterComponent characterComponent = ref CharacterComponent.ValueRW;
            ref CharacterControlComponent characterControlComponent = ref CharacterControl.ValueRW;
            ref quaternion characterRotation = ref CharacterDataAccess.LocalTransform.ValueRW.Rotation;

            KinematicCharacterUtilities.AddVariableRateRotationFromFixedRateRotation(ref characterRotation, characterBody.RotationFromParent, baseContext.Time.DeltaTime, characterBody.LastPhysicsUpdateDeltaTime);

            FirstPersonCharacterUtilities.ComputeFinalRotationsFromRotationDelta(
                ref characterRotation,
                ref characterComponent.ViewPitchDegrees,
                characterControlComponent.LookDegreesDelta,
                0f,
                characterComponent.MinViewAngle,
                characterComponent.MaxViewAngle,
                out float canceledPitchDegrees,
                out characterComponent.ViewLocalRotation);
        }

        #region Character Processor Callbacks

        public void UpdateGroundingUp(
            ref FirstPersonCharacterUpdateContext context,
            ref KinematicCharacterUpdateContext baseContext)
        {
            ref KinematicCharacterBody characterBody = ref CharacterDataAccess.CharacterBody.ValueRW;

            KinematicCharacterUtilities.Default_UpdateGroundingUp(
                ref characterBody,
                CharacterDataAccess.LocalTransform.ValueRO.Rotation);
        }

        public bool CanCollideWithHit(
            ref FirstPersonCharacterUpdateContext context,
            ref KinematicCharacterUpdateContext baseContext,
            in BasicHit hit)
        {
            return PhysicsUtilities.IsCollidable(hit.Material);
        }

        public bool IsGroundedOnHit(
            ref FirstPersonCharacterUpdateContext context,
            ref KinematicCharacterUpdateContext baseContext,
            in BasicHit hit,
            int groundingEvaluationType)
        {
            CharacterComponent characterComponent = CharacterComponent.ValueRO;

            return KinematicCharacterUtilities.Default_IsGroundedOnHit(
                in this,
                ref context,
                ref baseContext,
                CharacterDataAccess.CharacterEntity,
                CharacterDataAccess.PhysicsCollider.ValueRO,
                CharacterDataAccess.CharacterBody.ValueRO,
                CharacterDataAccess.CharacterProperties.ValueRO,
                in hit,
                in characterComponent.StepAndSlopeHandling,
                groundingEvaluationType);
        }

        public void OnMovementHit(
            ref FirstPersonCharacterUpdateContext context,
            ref KinematicCharacterUpdateContext baseContext,
            ref KinematicCharacterHit hit,
            ref float3 remainingMovementDirection,
            ref float remainingMovementLength,
            float3 originalVelocityDirection,
            float hitDistance)
        {
            ref KinematicCharacterBody characterBody = ref CharacterDataAccess.CharacterBody.ValueRW;
            ref float3 characterPosition = ref CharacterDataAccess.LocalTransform.ValueRW.Position;
            CharacterComponent characterComponent = CharacterComponent.ValueRO;

            KinematicCharacterUtilities.Default_OnMovementHit(
                in this,
                ref context,
                ref baseContext,
                ref characterBody,
                CharacterDataAccess.CharacterEntity,
                CharacterDataAccess.CharacterProperties.ValueRO,
                CharacterDataAccess.PhysicsCollider.ValueRO,
                CharacterDataAccess.LocalTransform.ValueRO,
                ref characterPosition,
                CharacterDataAccess.VelocityProjectionHits,
                ref hit,
                ref remainingMovementDirection,
                ref remainingMovementLength,
                originalVelocityDirection,
                hitDistance,
                characterComponent.StepAndSlopeHandling.StepHandling,
                characterComponent.StepAndSlopeHandling.MaxStepHeight,
                characterComponent.StepAndSlopeHandling.CharacterWidthForStepGroundingCheck);
        }

        public void OverrideDynamicHitMasses(
            ref FirstPersonCharacterUpdateContext context,
            ref KinematicCharacterUpdateContext baseContext,
            ref PhysicsMass characterMass,
            ref PhysicsMass otherMass,
            BasicHit hit)
        {
        }

        public void ProjectVelocityOnHits(
            ref FirstPersonCharacterUpdateContext context,
            ref KinematicCharacterUpdateContext baseContext,
            ref float3 velocity,
            ref bool characterIsGrounded,
            ref BasicHit characterGroundHit,
            in DynamicBuffer<KinematicVelocityProjectionHit> velocityProjectionHits,
            float3 originalVelocityDirection)
        {
            CharacterComponent characterComponent = CharacterComponent.ValueRO;

            KinematicCharacterUtilities.Default_ProjectVelocityOnHits(
                ref velocity,
                ref characterIsGrounded,
                ref characterGroundHit,
                in velocityProjectionHits,
                originalVelocityDirection,
                characterComponent.StepAndSlopeHandling.ConstrainVelocityToGroundPlane,
                in CharacterDataAccess.CharacterBody.ValueRO);
        }
        #endregion
    }
}