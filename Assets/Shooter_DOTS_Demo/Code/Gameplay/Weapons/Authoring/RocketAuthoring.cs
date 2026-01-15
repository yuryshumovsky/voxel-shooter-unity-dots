using Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components.Specific;
using Unity.Entities;
using UnityEngine;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Authoring
{
    [RequireComponent(typeof(ProjectileAuthoring))]
    public class RocketAuthoring : MonoBehaviour
    {
        public float DirectHitDamage;
        public float MaxRadiusDamage;
        public float DamageRadius;
        public float ExplosionSize = 1f;

        class Baker : Baker<RocketAuthoring>
        {
            public override void Bake(RocketAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new RocketComponent
                {
                    DirectHitDamage = authoring.DirectHitDamage,
                    MaxRadiusDamage = authoring.MaxRadiusDamage,
                    DamageRadius = authoring.DamageRadius,
                    ExplosionSize = authoring.ExplosionSize,
                });
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, DamageRadius);
        }
    }
}