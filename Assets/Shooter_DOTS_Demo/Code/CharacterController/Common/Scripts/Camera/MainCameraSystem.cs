using Unity.Entities;
using Unity.Transforms;

namespace Shooter_DOTS_Demo.Code.CharacterController.Common.Scripts.Camera
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class MainCameraSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            if (MainGameObjectCamera.Instance != null && SystemAPI.HasSingleton<MainEntityCamera>())
            {
                Entity mainEntityCameraEntity = SystemAPI.GetSingletonEntity<MainEntityCamera>();
                LocalToWorld targetLocalToWorld = SystemAPI.GetComponent<LocalToWorld>(mainEntityCameraEntity);
                MainGameObjectCamera.Instance.transform.SetPositionAndRotation(targetLocalToWorld.Position, targetLocalToWorld.Rotation);
            }
        }
    }
}
