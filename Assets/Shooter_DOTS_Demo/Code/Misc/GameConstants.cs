using Unity.Mathematics;

namespace Shooter_DOTS_Demo.Code.Infrastructure.States.GameStates
{
    public struct GameConstants
    {
        public static int ChunkSize = 16;
        public static int ExplosionsCapacity = 1000;
        
        public static readonly int3[] NeighborOffsets =
        {
            new int3(0, 1, 0),
            new int3(0, -1, 0),
            new int3(0, 0, 1),
            new int3(0, 0, -1),
            new int3(-1, 0, 0),
            new int3(1, 0, 0)
        };

        public static readonly int NeighborOffsetsCount = NeighborOffsets.Length;
    }
}