using Unity.Entities;
using Unity.Mathematics;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components
{
    public struct RaycastWeaponVisualProjectileBufferData : IBufferElementData
    {
        public uint Tick;
        public byte DidHit;
        public float3 EndPoint;
        public float3 HitNormal;
    }
}