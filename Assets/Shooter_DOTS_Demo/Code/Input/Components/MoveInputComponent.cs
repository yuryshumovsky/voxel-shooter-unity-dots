using Unity.Entities;
using Unity.Mathematics;

namespace Shooter_DOTS_Demo.Code.Input.Components
{
    public struct MoveInputComponent : IComponentData, IEnableableComponent
    {
        public float2 Value;
    }
}