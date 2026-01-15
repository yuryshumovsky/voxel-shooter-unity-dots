using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components
{
    public struct WeaponOwnerComponent : IComponentData
    {
        public Entity Entity;
    }
}