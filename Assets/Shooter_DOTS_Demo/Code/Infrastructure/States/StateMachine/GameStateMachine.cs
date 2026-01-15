using Shooter_DOTS_Demo.Code.Infrastructure.States.Factory;
using Shooter_DOTS_Demo.Code.Infrastructure.States.StateInfrastructure;
using Zenject;

namespace Shooter_DOTS_Demo.Code.Infrastructure.States.StateMachine
{
    public class GameStateMachine : IGameStateMachine, ITickable
    {
        private IExitableState _activeState;
        private readonly IStateFactory _stateFactory;

        public GameStateMachine(IStateFactory stateFactory)
        {
            _stateFactory = stateFactory;
        }

        public void Tick()
        {
            if (_activeState is IUpdateable updateableState)
                updateableState.Update();
        }

        public void Enter<TState>() where TState : class, IState
        {
            IState state = ChangeState<TState>();
            state.Enter();
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>
        {
            
            TState state = ChangeState<TState>();
            state.Enter(payload);
        }

        private TState ChangeState<TState>() where TState : class, IExitableState
        {
            _activeState?.Exit();
      
            TState state = _stateFactory.GetState<TState>();
            _activeState = state;
      
            return state;
        }
    }
}