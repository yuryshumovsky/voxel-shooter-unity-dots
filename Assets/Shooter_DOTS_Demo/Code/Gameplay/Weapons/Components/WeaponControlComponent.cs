using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components
{
    public struct WeaponControlComponent : IComponentData
    {
        public bool ShootPressed;
        public bool ShootReleased;
        public bool AimHeld;

        public uint InterpolationDelay;
    }
}