using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components
{
    public struct WeaponShotIgnoredEntityBufferData : IBufferElementData
    {
        public Entity Entity;
    }
}