using Shooter_DOTS_Demo.Code.CharacterController.Components;
using Shooter_DOTS_Demo.Code.CharacterController.Misc;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.CharacterController;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace Shooter_DOTS_Demo.Code.CharacterController.Systems
{
    [UpdateInGroup(typeof(KinematicCharacterPhysicsUpdateGroup))]
    [BurstCompile]
    public partial struct CharacterPhysicsUpdateSystem : ISystem
    {
        EntityQuery m_CharacterQuery;
        FirstPersonCharacterUpdateContext m_Context;
        KinematicCharacterUpdateContext m_BaseContext;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            m_CharacterQuery = KinematicCharacterUtilities.GetBaseCharacterQueryBuilder()
                .WithAll<CharacterComponent, CharacterControlComponent>()
                .Build(ref state);

            m_Context = new FirstPersonCharacterUpdateContext();
            m_Context.OnSystemCreate(ref state);
            m_BaseContext = new KinematicCharacterUpdateContext();
            m_BaseContext.OnSystemCreate(ref state);

            state.RequireForUpdate(m_CharacterQuery);
            state.RequireForUpdate<PhysicsWorldSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            m_Context.OnSystemUpdate(ref state);
            m_BaseContext.OnSystemUpdate(ref state, SystemAPI.Time, SystemAPI.GetSingleton<PhysicsWorldSingleton>());

            FirstPersonCharacterPhysicsUpdateJob job = new FirstPersonCharacterPhysicsUpdateJob
            {
                Context = m_Context,
                BaseContext = m_BaseContext,
            };
            job.ScheduleParallel();
        }

        [BurstCompile]
        [WithAll(typeof(Simulate))]
        public partial struct FirstPersonCharacterPhysicsUpdateJob : IJobEntity, IJobEntityChunkBeginEnd
        {
            public FirstPersonCharacterUpdateContext Context;
            public KinematicCharacterUpdateContext BaseContext;

            public void Execute(
                Entity entity,
                RefRW<LocalTransform> localTransform,
                RefRW<KinematicCharacterProperties> characterProperties,
                RefRW<KinematicCharacterBody> characterBody,
                RefRW<PhysicsCollider> physicsCollider,
                RefRW<CharacterComponent> characterComponent,
                RefRW<CharacterControlComponent> characterControl,
                DynamicBuffer<KinematicCharacterHit> characterHitsBuffer,
                DynamicBuffer<StatefulKinematicCharacterHit> statefulHitsBuffer,
                DynamicBuffer<KinematicCharacterDeferredImpulse> deferredImpulsesBuffer,
                DynamicBuffer<KinematicVelocityProjectionHit> velocityProjectionHits)
            {
                var characterProcessor = new FirstPersonCharacterProcessor()
                {
                    CharacterDataAccess = new KinematicCharacterDataAccess(

                        entity,
                        localTransform,
                        characterProperties,
                        characterBody,
                        physicsCollider,
                        characterHitsBuffer,
                        statefulHitsBuffer,
                        deferredImpulsesBuffer,
                        velocityProjectionHits
                    ),
                    CharacterComponent = characterComponent,
                    CharacterControl = characterControl
                };

                characterProcessor.PhysicsUpdate(ref Context, ref BaseContext);
            }

            public bool OnChunkBegin(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                BaseContext.EnsureCreationOfTmpCollections();
                return true;
            }

            public void OnChunkEnd(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask, bool chunkWasExecuted)
            {
            }
        }
    }
}