using Unity.Burst;
using Unity.Mathematics;
using Shooter_DOTS_Demo.Code.Infrastructure.States.GameStates;

namespace Shooter_DOTS_Demo.Code.Utils
{
    public struct PositionUtils
    {
        public static int IndexFromXYZ(int x, int y, int z, int size)
        {
            return x + y * size + z * size * size;
        }
        
        public static int3 XYZFromIndex(int index, int size)
        {
            int z = index / (size * size);
            int remainder = index % (size * size);
            int y = remainder / size;
            int x = remainder % size;
    
            return new int3(x, y, z);
        }
        
        [BurstCompile]
        public static int3 GetChunkPositionFromWorldPosition(float3 worldPosition)
        {
            int chunkX = (int)math.floor(worldPosition.x / GameConstants.ChunkSize);
            int chunkY = (int)math.floor(worldPosition.y / GameConstants.ChunkSize);
            int chunkZ = (int)math.floor(worldPosition.z / GameConstants.ChunkSize);
            return new int3(
                chunkX * GameConstants.ChunkSize,
                chunkY * GameConstants.ChunkSize,
                chunkZ * GameConstants.ChunkSize
            );
        }
        
        [BurstCompile]
        public static int3 GetChunkPositionFromWorldPosition(int3 worldPosition)
        {
            return GetChunkPositionFromWorldPosition((float3)worldPosition);
        }
        
        [BurstCompile]
        public static int3 GetVoxelPositionFromWorldPosition(float3 worldPosition)
        {
            return new int3(
                (int)math.round(worldPosition.x),
                (int)math.round(worldPosition.y),
                (int)math.round(worldPosition.z)
            );
        }
        
        [BurstCompile]
        public static float3 GetWorldPositionFromVoxelPosition(int3 voxelPosition)
        {
            return (float3)voxelPosition;
        }
    }
    
    
}