using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components
{
    public struct BaseWeaponComponent : IComponentData
    {
        public Entity ShotOrigin;
        public bool Automatic;
        public float FiringRate;
        public float SpreadRadians;
        public int ProjectilesPerShot;

        public float ShotTimer;
        public bool IsFiring;
        public uint TotalShotsCount;
        public uint TotalProjectilesCount;

        public byte LastVisualTotalShotsCountInitialized;
        public uint LastVisualTotalShotsCount;
        public byte LastVisualTotalProjectilesCountInitialized;
        public uint LastVisualTotalProjectilesCount;
    }
}