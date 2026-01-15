using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components
{
    public struct WeaponShotStartPointComponent : IComponentData
    {
        public Entity Entity;
    }
}