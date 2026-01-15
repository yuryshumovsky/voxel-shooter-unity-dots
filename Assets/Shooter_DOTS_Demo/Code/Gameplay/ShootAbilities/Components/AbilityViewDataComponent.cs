using Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.Enums;
using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.Components
{
    public struct AbilityViewDataComponent : IComponentData
    {
        public bool enableToUse;
        public float progress;
        public bool showCount;
        public bool availableCount;
        public ShootAbilityType type;
        public bool selected;
    }
}