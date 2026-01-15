using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Shooter_DOTS_Demo.Code.Gameplay.Map.Components.Singletons
{
    public struct VoxelMapSingletonComponent : IComponentData
    {
        public NativeParallelHashMap<int3, Entity> VoxelsMap;
    }
}

