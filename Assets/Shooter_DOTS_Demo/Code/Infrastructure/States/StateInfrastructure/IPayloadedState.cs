namespace Shooter_DOTS_Demo.Code.Infrastructure.States.StateInfrastructure
{
    public interface IPayloadedState<TPayload> : IExitableState
    {
        void Enter(TPayload payload);
    }
}