using Cysharp.Threading.Tasks;
using Shooter_DOTS_Demo.Code.Infrastructure.Loading;
using Shooter_DOTS_Demo.Code.Infrastructure.States.GameStates.Components.StateTag;
using Shooter_DOTS_Demo.Code.Infrastructure.States.StateInfrastructure;
using Shooter_DOTS_Demo.Code.Infrastructure.States.StateMachine;
using Shooter_DOTS_Demo.Code.Utils;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Shooter_DOTS_Demo.Code.Infrastructure.States.GameStates
{
    public class LoadingGameState : IPayloadedState<string>
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly ISceneLoader _sceneLoader;
        private Entity _stateTagEntity;

        public LoadingGameState(IGameStateMachine stateMachine, ISceneLoader sceneLoader)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
        }

        public void Enter(string sceneName)
        {
            _stateTagEntity = EntityUtils.CreateEntity()
                    .AddComponent(new LoadingGameStateTagComponent())
                    .SetName("state_loading_game")
                ;

            LoadGameScene(sceneName);
        }

        private async UniTask LoadGameScene(string sceneName)
        {
            bool success = await _sceneLoader.LoadSceneAsync(
                sceneName: sceneName,
                loadMode: LoadSceneMode.Single,
                onProgress: null);

            if (success)
            {
                EnterBattleLoopState();
            }

            if (!success)
            {
                Debug.LogError($"Scene '{sceneName}' load error");
            }
        }

        private void EnterBattleLoopState()
        {
            _stateMachine.Enter<GamePrepareState>();
        }

        public void Exit()
        {
            _stateTagEntity.Destroy();
        }
    }
}