using Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components;
using Unity.Entities;
using UnityEngine;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Authoring
{
    [RequireComponent(typeof(BaseWeaponAuthoring))]
    public class WeaponProjectileReferenceAuthoring : MonoBehaviour
    {
        public GameObject ProjectilePrefab;

        class Baker : Baker<WeaponProjectileReferenceAuthoring>
        {
            public override void Bake(WeaponProjectileReferenceAuthoring referenceAuthoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new WeaponProjectileReferenceComponent()
                {
                    ProjectilePrefab = GetEntity(referenceAuthoring.ProjectilePrefab, TransformUsageFlags.Dynamic),
                });
            }
        }
    }
}