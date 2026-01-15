using Unity.Collections;
using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Gameplay.Players.Components
{
    public struct PlayerNameComponent : IComponentData
    {
        public FixedString64Bytes Value;
    }
}