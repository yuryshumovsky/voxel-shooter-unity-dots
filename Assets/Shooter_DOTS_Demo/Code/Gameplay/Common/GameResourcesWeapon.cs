using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Gameplay.Common
{
    public struct GameResourcesWeapon : IBufferElementData
    {
        public Entity WeaponPrefab;
    }
}