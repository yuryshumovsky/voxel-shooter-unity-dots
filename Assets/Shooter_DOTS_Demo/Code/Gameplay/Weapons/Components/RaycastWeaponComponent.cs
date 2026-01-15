using Shooter_DOTS_Demo.Code.Gameplay.Weapons.Enums;
using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components
{
    public struct RaycastWeaponComponent : IComponentData
    {
        public Entity ProjectilePrefab;
        public RaycastWeaponVisualsSyncMode VisualsSyncMode;

        public uint LastProcessedProjectileVisualEventTick;
    }
}