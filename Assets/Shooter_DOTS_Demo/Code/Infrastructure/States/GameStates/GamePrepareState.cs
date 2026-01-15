using System;
using System.Collections.Generic;
using Shooter_DOTS_Demo.Code.Gameplay.Map.Components;
using Shooter_DOTS_Demo.Code.Gameplay.Map.Services;
using Shooter_DOTS_Demo.Code.Gameplay.Players.Components;
using Shooter_DOTS_Demo.Code.Infrastructure.States.GameStates.Components;
using Shooter_DOTS_Demo.Code.Infrastructure.States.GameStates.Components.StateTag;
using Shooter_DOTS_Demo.Code.Infrastructure.States.StateInfrastructure;
using Shooter_DOTS_Demo.Code.Infrastructure.States.StateMachine;
using Shooter_DOTS_Demo.Code.Utils;
using Unity.Entities;
using Unity.Mathematics;
using Zenject;

namespace Shooter_DOTS_Demo.Code.Infrastructure.States.GameStates
{
    public class GamePrepareState : IState, ITickable
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly MapLoadingService _mapLoadingService;
        private readonly MapVoxelsAndChunksInitializeService _mapVoxelsAndChunksInitializeService;
        private Entity _stateTagEntity;
        private Entity _prepareStateDetectorEntity;
        private bool _isActive;

        public GamePrepareState(
            IGameStateMachine stateMachine,
            MapLoadingService mapLoadingService,
            MapVoxelsAndChunksInitializeService mapVoxelsAndChunksInitializeService
        )
        {
            _stateMachine = stateMachine;
            _mapLoadingService = mapLoadingService;
            _mapVoxelsAndChunksInitializeService = mapVoxelsAndChunksInitializeService;
        }

        public void Exit()
        {
            _isActive = false;
            _stateTagEntity.Destroy();
            _prepareStateDetectorEntity.Destroy();
        }

        public void Enter()
        {
            _isActive = true;
            _stateTagEntity = EntityUtils.CreateEntity()
                    .AddComponent(new GamePrepareStateTagComponent())
                    .SetName("state_prepare_game")
                ;

            Dictionary<int3, byte> voxels = _mapLoadingService.LoadVoxelMap("voxel_city");

            var chunkSize = GameConstants.ChunkSize;
            if (voxels != null)
            {
                Dictionary<int3, byte> debugDict = new Dictionary<int3, byte>();

                int i = 0;
                int type = 0;
                for (int x = 0; x < 150; x++)
                {
                    for (int y = 0; y < 150; y++)
                    {
                        for (int z = 0; z < 150; z++)
                        {
                            i++;
                            if (i > 1300)
                            {
                                type++;
                                i = 0;
                            }

                            debugDict.Add(new int3(x, y, z), (byte)(type % 4));
                        }
                    }
                }

                _mapVoxelsAndChunksInitializeService.InitializeChunksFromVoxels(voxels, chunkSize);
            }
            else
            {
                throw new Exception("Error: Can't get map voxels");
            }

            _prepareStateDetectorEntity = EntityUtils.CreateEntity();
            _prepareStateDetectorEntity
                .SetName("prepare_state_detector_entity")
                .AddComponent(new MapBuildDoneTagComponent())
                .SetComponentEnabled<MapBuildDoneTagComponent>(false)
                ;

            EntityUtils
                .CreateEntity()
                .AddComponent(new InitializeGroundedVoxelsRequestComponent());

            EntityUtils
                .CreateEntity()
                .AddComponent(new SpawnPlayerRequest());
        }

        public void Tick()
        {
            if (!_isActive)
                return;

            if (_prepareStateDetectorEntity.IsComponentEnabled<MapBuildDoneTagComponent>())
            {
                _stateMachine.Enter<GameLoopState>();
            }
        }
    }
}