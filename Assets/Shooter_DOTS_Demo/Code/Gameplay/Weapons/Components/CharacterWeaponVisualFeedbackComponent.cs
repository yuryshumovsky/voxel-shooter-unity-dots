using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components
{
    [Serializable]
    public struct CharacterWeaponVisualFeedbackComponent : IComponentData
    {
        public float3 WeaponLocalPosBob;
        public float3 WeaponLocalPosRecoil;

        public float CurrentRecoil;

        public float TargetRecoilFOVKick;
        public float CurrentRecoilFOVKick;
    }
}