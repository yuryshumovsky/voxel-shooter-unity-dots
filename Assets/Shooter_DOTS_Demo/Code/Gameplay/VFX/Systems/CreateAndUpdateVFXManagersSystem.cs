using Shooter_DOTS_Demo.Code.Gameplay.VFX.Components;
using Shooter_DOTS_Demo.Code.Infrastructure.States.GameStates;
using Shooter_DOTS_Demo.Code.Infrastructure.States.GameStates.Components.StateTag;
using Unity.Entities;
using UnityEngine;

namespace Shooter_DOTS_Demo.Code.Gameplay.VFX.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    public partial struct CreateAndUpdateVFXManagersSystem : ISystem
    {
        int _spawnBatchId;
        int _requestsCountId;
        int _requestsBufferId;

        VFXManager<VFXExplosionRequest> _explosionsManager;
        
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameLoopStateTagComponent>();
            
            _spawnBatchId = Shader.PropertyToID("SpawnBatch");
            _requestsCountId = Shader.PropertyToID("SpawnRequestsCount");
            _requestsBufferId = Shader.PropertyToID("SpawnRequestsBuffer");

            _explosionsManager = new VFXManager<VFXExplosionRequest>(GameConstants.ExplosionsCapacity);

            state.EntityManager.AddComponentData(
                state.EntityManager.CreateEntity(),
                new VFXExplosionsManagerSingletonComponent
                {
                    Manager = _explosionsManager,
                }
            );
        }

        public void OnDestroy(ref SystemState state)
        {
            _explosionsManager.Dispose();
        }

        private static bool _hasStartedGraph = false;

        public void OnUpdate(ref SystemState state)
        {
            SystemAPI
                .QueryBuilder()
                .WithAll<VFXExplosionsManagerSingletonComponent>()
                .Build()
                .CompleteDependency();

            var vfxExplosionsManagerSingleton = SystemAPI
                .GetSingletonRW<VFXExplosionsManagerSingletonComponent>();
            
            ref var manager = ref vfxExplosionsManagerSingleton.ValueRW.Manager;

            if (VFXReferences.ExplosionsGraph == null)
            {
                return;
            }

            if (VFXReferences.ExplosionsRequestsBuffer == null)
            {
                return;
            }

            if (!_hasStartedGraph)
            {
                if (!VFXReferences.ExplosionsGraph.enabled)
                {
                    VFXReferences.ExplosionsGraph.enabled = true;
                }

                VFXReferences.ExplosionsGraph.Play();
                _hasStartedGraph = true;
            }

            float rateRatio = SystemAPI.Time.DeltaTime / Time.deltaTime;
            bool shouldUploadVFXData = true;

            manager.Update(
                VFXReferences.ExplosionsGraph,
                ref VFXReferences.ExplosionsRequestsBuffer,
                shouldUploadVFXData,
                rateRatio,
                _spawnBatchId,
                _requestsCountId,
                _requestsBufferId
            );
        }
    }
}