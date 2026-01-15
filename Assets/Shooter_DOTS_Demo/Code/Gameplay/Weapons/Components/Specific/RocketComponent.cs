using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components.Specific
{
    public struct RocketComponent : IComponentData, IEnableableComponent
    {
        public float DirectHitDamage;
        public float MaxRadiusDamage;
        public float DamageRadius;
        public float ExplosionSize;

        public byte HasProcessedHitSimulation;
        public byte HasProcessedHitVFX;
    }
}