using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components.Projectile
{
    public struct ProjectileSpawnIdComponent : IComponentData
    {
        public Entity WeaponEntity;
        public uint SpawnId;

        public bool IsSame(ProjectileSpawnIdComponent other)
        {
            return WeaponEntity == other.WeaponEntity && SpawnId == other.SpawnId;
        }
    }
}