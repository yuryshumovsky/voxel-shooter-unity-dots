using Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components.Projectile;
using Unity.Entities;
using UnityEngine;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Authoring
{
    class RaycastProjectileAuthoring : MonoBehaviour
    {
        public float Damage = 1f;
        public float Range = 1000f;

        class Baker : Baker<RaycastProjectileAuthoring>
        {
            public override void Bake(RaycastProjectileAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new RaycastProjectileComponent
                {
                    Damage = authoring.Damage,
                    Range = authoring.Range,
                });
                AddComponent(entity, new RaycastVisualProjectileComponent());
            }
        }
    }
}