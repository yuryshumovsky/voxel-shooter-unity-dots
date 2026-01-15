using Shooter_DOTS_Demo.Code.Infrastructure.States.StateInfrastructure;

namespace Shooter_DOTS_Demo.Code.Infrastructure.States.Factory
{
    public interface IStateFactory
    {
        T GetState<T>() where T : class, IExitableState;
    }
}