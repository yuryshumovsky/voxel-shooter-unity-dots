using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.Components
{
    public struct AbilityCooldownComponent : IComponentData, IEnableableComponent
    {
        public float timer;
        public float timerMax;
        public bool done;
    }
}