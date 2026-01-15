using Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.Enums;
using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.Components
{
    [InternalBufferCapacity(4)]
    public struct AbilitySpawnBuffer : IBufferElementData {
        public ShootAbilityType type;
        public bool viewAdded;
    }
}