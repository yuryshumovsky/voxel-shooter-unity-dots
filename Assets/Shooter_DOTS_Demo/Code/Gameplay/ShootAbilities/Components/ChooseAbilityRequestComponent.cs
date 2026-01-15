using Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.Enums;
using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.Components
{
    public struct ChooseAbilityRequestComponent : IComponentData
    {
        public ShootAbilityType type;
    }
}