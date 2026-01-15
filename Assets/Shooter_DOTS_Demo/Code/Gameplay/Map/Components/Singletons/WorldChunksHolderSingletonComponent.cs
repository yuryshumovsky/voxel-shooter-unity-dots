using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Shooter_DOTS_Demo.Code.Gameplay.Map.Components.Singletons
{
    public struct ChunksMapSingletonComponent : IComponentData
    {
        public NativeHashMap<int3, Entity> ChunksMap;
    }
}