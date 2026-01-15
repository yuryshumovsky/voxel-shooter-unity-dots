using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components
{
    public struct ActiveWeaponComponent : IComponentData
    {
        public Entity CurrentWeaponEntity;
        public Entity PreviousWeaponEntity;
    }
}