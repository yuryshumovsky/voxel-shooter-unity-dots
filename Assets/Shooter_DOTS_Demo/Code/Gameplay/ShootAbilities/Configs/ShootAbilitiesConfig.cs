using System;
using System.Collections.Generic;
using Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.Enums;
using UnityEngine;

namespace Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.Configs
{
    [CreateAssetMenu(menuName = "DOTS Demo/Create shoot abilities config", fileName = "ShootAbilitiesConfig")]
    public class ShootAbilitiesConfig : ScriptableObject
    {
        public List<ShootAbilityConfig> shootAbilities;

        public ShootAbilityConfig GetConfigByType(ShootAbilityType shootAbilityType)
        {
            foreach (var config in shootAbilities)
            {
                if (config.shootAbilityType == shootAbilityType)
                {
                    return config;
                }
            }

            Debug.LogError($"Could not find ShootAbilityConfig for type {shootAbilityType}");
            return null;
        }
    }

    [Serializable]
    public class ShootAbilityConfig
    {
        public ShootAbilityType shootAbilityType;
        public float cooldown;
        public Sprite buttonIcon;
    }
}