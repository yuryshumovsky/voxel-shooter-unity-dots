using Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components;
using Shooter_DOTS_Demo.Code.Gameplay.Weapons.Enums;
using Unity.Entities;
using UnityEngine;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Authoring
{
    [RequireComponent(typeof(BaseWeaponAuthoring))]
    public class RaycastProjectileWeaponAuthoring : MonoBehaviour
    {
        public GameObject ProjectilePrefab;
        public RaycastWeaponVisualsSyncMode VisualsSyncMode;

        public class Baker : Baker<RaycastProjectileWeaponAuthoring>
        {
            public override void Bake(RaycastProjectileWeaponAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new RaycastWeaponComponent()
                {
                    ProjectilePrefab = GetEntity(authoring.ProjectilePrefab, TransformUsageFlags.Dynamic),
                    VisualsSyncMode = authoring.VisualsSyncMode,
                });
                AddBuffer<RaycastWeaponVisualProjectileBufferData>(entity);
            }
        }
    }
}