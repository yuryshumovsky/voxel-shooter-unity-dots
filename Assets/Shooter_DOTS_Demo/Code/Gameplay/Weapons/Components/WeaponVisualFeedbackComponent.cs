using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components
{
    [Serializable]
    public struct WeaponVisualFeedbackComponent : IComponentData
    {
        [Serializable]
        public struct Authoring
        {
            [Header("Bobbing")] public float WeaponBobHAmount;
            public float WeaponBobVAmount;
            public float WeaponBobFrequency;
            public float WeaponBobSharpness;
            public float WeaponBobAimRatio;

            [Header("Recoil")] public float RecoilStrength;
            public float RecoilMaxDistance;
            public float RecoilSharpness;
            public float RecoilRestitutionSharpness;

            [Header("Aiming")] public float AimFOVRatio;
            public float AimFOVSharpness;
            public float LookSensitivityMultiplierWhileAiming;

            [Header("FoV Kick")] public float RecoilFOVKick;
            public float RecoilMaxFOVKick;
            public float RecoilFOVKickSharpness;
            public float RecoilFOVKickRestitutionSharpness;

            public static Authoring GetDefault()
            {
                return new Authoring
                {
                    WeaponBobHAmount = 0.08f,
                    WeaponBobVAmount = 0.06f,
                    WeaponBobFrequency = 10f,
                    WeaponBobSharpness = 10f,
                    WeaponBobAimRatio = 0.25f,

                    RecoilStrength = 1f,
                    RecoilMaxDistance = 0.5f,
                    RecoilSharpness = 100f,
                    RecoilRestitutionSharpness = 5f,

                    AimFOVRatio = 0.5f,
                    AimFOVSharpness = 10f,
                    LookSensitivityMultiplierWhileAiming = 0.7f,

                    RecoilFOVKick = 10f,
                    RecoilMaxFOVKick = 10f,
                    RecoilFOVKickSharpness = 150f,
                    RecoilFOVKickRestitutionSharpness = 15f,
                };
            }
        }

        public WeaponVisualFeedbackComponent(Authoring authoring)
        {
            WeaponBobHAmount = authoring.WeaponBobHAmount;
            WeaponBobVAmount = authoring.WeaponBobVAmount;
            WeaponBobFrequency = authoring.WeaponBobFrequency;
            WeaponBobSharpness = authoring.WeaponBobSharpness;
            WeaponBobAimRatio = authoring.WeaponBobAimRatio;

            RecoilStrength = authoring.RecoilStrength;
            RecoilMaxDistance = authoring.RecoilMaxDistance;
            RecoilSharpness = authoring.RecoilSharpness;
            RecoilRestitutionSharpness = authoring.RecoilRestitutionSharpness;

            AimFOVRatio = authoring.AimFOVRatio;
            AimFOVSharpness = authoring.AimFOVSharpness;
            LookSensitivityMultiplierWhileAiming = authoring.LookSensitivityMultiplierWhileAiming;

            RecoilFOVKick = authoring.RecoilFOVKick;
            RecoilMaxFOVKick = authoring.RecoilMaxFOVKick;
            RecoilFOVKickSharpness = authoring.RecoilFOVKickSharpness;
            RecoilFOVKickRestitutionSharpness = authoring.RecoilFOVKickRestitutionSharpness;

            WeaponLocalPosRecoil = float3.zero;
            CurrentRecoil = 0f;
            BaseLocalPosition = float3.zero;
            BasePositionInitialized = 0;
        }

        public float WeaponBobHAmount;
        public float WeaponBobVAmount;
        public float WeaponBobFrequency;
        public float WeaponBobSharpness;
        public float WeaponBobAimRatio;

        public float RecoilStrength;
        public float RecoilMaxDistance;
        public float RecoilSharpness;
        public float RecoilRestitutionSharpness;

        public float AimFOVRatio;
        public float AimFOVSharpness;
        public float LookSensitivityMultiplierWhileAiming;

        public float RecoilFOVKick;
        public float RecoilMaxFOVKick;
        public float RecoilFOVKickSharpness;
        public float RecoilFOVKickRestitutionSharpness;

        public float3 WeaponLocalPosRecoil;
        public float CurrentRecoil;
        public float3 BaseLocalPosition;
        public byte BasePositionInitialized;
    }
}