using Unity.Entities;
using UnityEngine;

namespace Shooter_DOTS_Demo.Code.CharacterController.Common.Scripts.Camera
{
    [DisallowMultipleComponent]
    public class MainEntityCameraAuthoring : MonoBehaviour
    {
        public class Baker : Baker<MainEntityCameraAuthoring>
        {
            public override void Bake(MainEntityCameraAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<MainEntityCamera>(entity);
            }
        }
    }
}
