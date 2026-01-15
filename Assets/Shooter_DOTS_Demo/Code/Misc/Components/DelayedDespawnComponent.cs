using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Misc
{
    public struct DelayedDespawnComponent : IComponentData, IEnableableComponent
    {
        public uint DespawnTicks;
        public uint Ticks;
        public byte HasHandledPreDespawn;
    }
}