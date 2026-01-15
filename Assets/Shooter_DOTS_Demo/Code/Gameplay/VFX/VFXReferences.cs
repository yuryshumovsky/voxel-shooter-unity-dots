using UnityEngine;
using UnityEngine.VFX;

namespace Shooter_DOTS_Demo.Code.Gameplay.VFX
{
    public static class VFXReferences
    {
        public static VisualEffect ExplosionsGraph;
        public static GraphicsBuffer ExplosionsRequestsBuffer;

        public static void LogVFXState()
        {
            Debug.Log("[VFX] ===== VFX References State =====");
            
            if (ExplosionsGraph == null)
            {
                Debug.LogError("[VFX] ExplosionsGraph: NULL");
            }
            else
            {
                Debug.Log($"[VFX] ExplosionsGraph: " +
                    $"GameObject: {(ExplosionsGraph.gameObject != null ? ExplosionsGraph.gameObject.name : "null")}, " +
                    $"Enabled: {ExplosionsGraph.enabled}, " +
                    $"PlayRate: {ExplosionsGraph.playRate}");
            }

            if (ExplosionsRequestsBuffer == null)
            {
                Debug.LogError("[VFX] ExplosionsRequestsBuffer: NULL");
            }
            else
            {
                Debug.Log($"[VFX] ExplosionsRequestsBuffer: " +
                    $"IsValid: {ExplosionsRequestsBuffer.IsValid()}, " +
                    $"Count: {ExplosionsRequestsBuffer.count}, " +
                    $"Stride: {ExplosionsRequestsBuffer.stride}");
            }

            Debug.Log("[VFX] ===============================");
        }
    }
}