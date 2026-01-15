using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components.Projectile
{
    public struct RaycastProjectileComponent : IComponentData
    {
        public float Range;
        public float Damage;
    }
}