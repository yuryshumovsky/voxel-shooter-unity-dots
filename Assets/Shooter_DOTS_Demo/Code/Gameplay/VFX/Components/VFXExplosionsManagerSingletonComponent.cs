using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Gameplay.VFX.Components
{
    public struct VFXExplosionsManagerSingletonComponent : IComponentData
    {
        public VFXManager<VFXExplosionRequest> Manager;
    }
}