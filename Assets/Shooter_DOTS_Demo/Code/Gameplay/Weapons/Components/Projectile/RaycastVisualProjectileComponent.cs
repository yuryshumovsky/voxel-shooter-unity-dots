using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components.Projectile
{
    [Serializable]
    public struct RaycastVisualProjectileComponent : IComponentData
    {
        public byte DidHit;
        public float3 StartPoint;
        public float3 EndPoint;
        public float3 HitNormal;

        public float GetLengthOfTrajectory()
        {
            return math.length(EndPoint - StartPoint);
        }

        public float3 GetDirection()
        {
            return math.normalizesafe(EndPoint - StartPoint);
        }
    }
}