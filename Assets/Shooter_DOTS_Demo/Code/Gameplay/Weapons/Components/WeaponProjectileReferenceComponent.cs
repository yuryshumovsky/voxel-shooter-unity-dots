using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components
{
    public struct WeaponProjectileReferenceComponent : IComponentData
    {
        public Entity ProjectilePrefab;
    }
}