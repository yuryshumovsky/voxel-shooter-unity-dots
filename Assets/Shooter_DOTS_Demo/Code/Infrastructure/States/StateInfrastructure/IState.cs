namespace Shooter_DOTS_Demo.Code.Infrastructure.States.StateInfrastructure
{
    public interface IState : IExitableState
    {
        void Enter();
        static int ChunkSize { get; }
    }
}