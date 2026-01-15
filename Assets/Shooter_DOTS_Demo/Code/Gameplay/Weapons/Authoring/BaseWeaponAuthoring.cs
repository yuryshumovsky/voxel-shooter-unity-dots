using Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Authoring
{
    class BaseWeaponAuthoring : MonoBehaviour
    {
        public GameObject ShotOrigin;
        public bool Automatic;
        public float FiringRate;
        public float SpreadDegrees;
        public int ProjectilesPerShot;
        public WeaponVisualFeedbackComponent.Authoring VisualFeedback = WeaponVisualFeedbackComponent.Authoring.GetDefault();

        class Baker : Baker<BaseWeaponAuthoring>
        {
            public override void Bake(BaseWeaponAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new BaseWeaponComponent()
                {
                    ShotOrigin = GetEntity(authoring.ShotOrigin, TransformUsageFlags.Dynamic),
                    Automatic = authoring.Automatic,
                    FiringRate = authoring.FiringRate,
                    SpreadRadians = math.radians(authoring.SpreadDegrees),
                    ProjectilesPerShot = authoring.ProjectilesPerShot,
                });
                AddComponent(entity, new WeaponVisualFeedbackComponent(authoring.VisualFeedback));
                AddComponent(entity, new WeaponControlComponent());
                AddComponent(entity, new WeaponOwnerComponent());
                AddComponent(entity, new WeaponShotStartPointComponent());
                AddBuffer<WeaponShotIgnoredEntityBufferData>(entity);
                AddBuffer<WeaponProjectileWaitToSpawnBufferData>(entity);
            }
        }
    }
}