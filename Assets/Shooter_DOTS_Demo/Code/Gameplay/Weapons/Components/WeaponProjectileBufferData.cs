using Unity.Entities;
using Unity.Mathematics;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components
{
    public struct WeaponProjectileWaitToSpawnBufferData : IBufferElementData
    {
        public uint ID;
        public float3 SimulationPosition;
        public float3 VisualPosition;
        public float3 SimulationDirection;
        public quaternion SimulationRotation;
    }
}