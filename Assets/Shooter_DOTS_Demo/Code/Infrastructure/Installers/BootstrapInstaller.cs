using Shooter_DOTS_Demo.Code.Gameplay.Map.Services;
using Shooter_DOTS_Demo.Code.Infrastructure.Loading;
using Shooter_DOTS_Demo.Code.Infrastructure.States.Factory;
using Shooter_DOTS_Demo.Code.Infrastructure.States.GameStates;
using Shooter_DOTS_Demo.Code.Infrastructure.States.StateMachine;
using Shooter_DOTS_Demo.Code.Input.Services;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Shooter_DOTS_Demo.Code.Infrastructure.Installers
{
    public class BootstrapInstaller : MonoInstaller, IInitializable
    {
        [SerializeField] private InputActionAsset _inputActionAsset;

        public override void InstallBindings()
        {
            Application.targetFrameRate = 60;

            BindInfrastructureServices();
            BindInputServices();
            BindStateMachine();
            BindStateFactory();
            BindGameStates();
            BindGameplayServices();
        }

        private void BindInputServices()
        {
            Container.BindInstance(_inputActionAsset);
            Container.Bind<IInputService>().To<StandaloneInputService>().AsSingle();
            Container.BindInterfacesAndSelfTo<EmitInputToECSWorldService>().AsSingle().Lazy();
        }

        private void BindGameplayServices()
        {
            Container.BindInterfacesAndSelfTo<MapLoadingService>().AsSingle();
            Container.BindInterfacesAndSelfTo<MapVoxelsAndChunksInitializeService>().AsSingle();
        }

        private void BindInfrastructureServices()
        {
            Container.BindInterfacesTo<BootstrapInstaller>().FromInstance(this).AsSingle();
            Container.Bind<ISceneLoader>().To<SceneLoader>().AsSingle();
        }

        private void BindStateFactory()
        {
            Container.BindInterfacesAndSelfTo<StateFactory>().AsSingle();
        }

        private void BindStateMachine()
        {
            Container.BindInterfacesAndSelfTo<GameStateMachine>().AsSingle();
        }

        private void BindGameStates()
        {
            Container.BindInterfacesAndSelfTo<BootstrapState>().AsSingle();
            Container.BindInterfacesAndSelfTo<LoadingGameState>().AsSingle();
            Container.BindInterfacesAndSelfTo<GamePrepareState>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameLoopState>().AsSingle();
        }


        public void Initialize()
        {
            Container.Resolve<IGameStateMachine>().Enter<BootstrapState>();
        }
    }
}