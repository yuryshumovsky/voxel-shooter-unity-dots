using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.Components
{
    public struct AbilityOwnerComponent : IComponentData
    {
        public Entity Value;
    }
}