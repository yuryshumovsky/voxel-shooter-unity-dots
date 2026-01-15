using Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components;
using Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components.Projectile;
using Unity.Entities;
using UnityEngine;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Authoring
{
    class ProjectileAuthoring : MonoBehaviour
    {
        public float Speed = 10f;
        public float Gravity = -10f;
        public float MaxLifetime = 5f;
        public float VisualOffsetCorrectionDuration = 0.3f;

        class Baker : Baker<ProjectileAuthoring>
        {
            public override void Bake(ProjectileAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ProjectileComponent
                {
                    Speed = authoring.Speed,
                    Gravity = authoring.Gravity,
                    MaxLifetime = authoring.MaxLifetime,
                    VisualOffsetCorrectionDuration = authoring.VisualOffsetCorrectionDuration,
                    LifetimeCounter = 0f,
                });
                AddComponent(entity, new ProjectileSpawnIdComponent());
                AddComponent<WeaponShotIgnoredEntityBufferData>(entity);
            }
        }
    }
}