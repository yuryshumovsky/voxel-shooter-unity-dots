using Shooter_DOTS_Demo.Code.Gameplay.Common;
using Shooter_DOTS_Demo.Code.Gameplay.Map.Configs;
using Shooter_DOTS_Demo.Code.Gameplay.Players.Configs;
using Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.Configs;
using Shooter_DOTS_Demo.Code.Infrastructure.States.GameStates.Components.StateTag;
using Shooter_DOTS_Demo.Code.Infrastructure.States.StateInfrastructure;
using Shooter_DOTS_Demo.Code.Infrastructure.States.StateMachine;
using Shooter_DOTS_Demo.Code.Utils;
using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Infrastructure.States.GameStates
{
    public class BootstrapState : IState
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly FigureConfig _figureConfig;
        private readonly PlayersConfig _playersConfig;
        private readonly ShootAbilitiesConfig _shootAbilitiesConfig;

        private Entity _stateTagEntity;

        private const string GameSceneName = "Game";

        public BootstrapState(
            IGameStateMachine stateMachine,
            FigureConfig figureConfig,
            PlayersConfig playersConfig,
            ShootAbilitiesConfig shootAbilitiesConfig
        )
        {
            _stateMachine = stateMachine;
            _figureConfig = figureConfig;
            _playersConfig = playersConfig;
            _shootAbilitiesConfig = shootAbilitiesConfig;
           
        }

        public void Enter()
        {
            _stateTagEntity = EntityUtils
                    .CreateEntity()
                    .AddComponent(new BootstrapStateTagComponent())
                    .SetName("state_bootstrap")
                ;

            Entity configHolderEntity = EntityUtils.CreateEntity();
            configHolderEntity.AddComponent(new GameSettingsHolderComponent()
            {
                figureConfig = _figureConfig,
                playersConfig = _playersConfig,
                shootAbilitiesConfig = _shootAbilitiesConfig
            });

            _stateMachine.Enter<LoadingGameState, string>(GameSceneName);
        }

        public void Exit()
        {
            _stateTagEntity.Destroy();
        }
    }
}