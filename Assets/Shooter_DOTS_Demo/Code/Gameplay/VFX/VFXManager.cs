using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;

namespace Shooter_DOTS_Demo.Code.Gameplay.VFX
{
    public struct VFXManager<T> where T : unmanaged
    {
        public NativeReference<int> RequestsCount;
        public NativeArray<T> Requests;

        public bool GraphIsInitialized { get; private set; }

        public VFXManager(int maxRequests)
        {
            RequestsCount = new NativeReference<int>(0, Allocator.Persistent);
            Requests = new NativeArray<T>(maxRequests, Allocator.Persistent);

            GraphIsInitialized = false;
        }

        public void Dispose()
        {
            if (RequestsCount.IsCreated)
            {
                RequestsCount.Dispose();
            }

            if (Requests.IsCreated)
            {
                Requests.Dispose();
            }
        }

        public void Update(
            VisualEffect vfxGraph,
            ref GraphicsBuffer graphicsBuffer,
            bool uploadDataToGraphs,
            float deltaTimeMultiplier,
            int spawnBatchId,
            int requestsCountId,
            int requestsBufferId)
        {
            if (vfxGraph == null)
            {
                Debug.LogError("[VFX] VFXManager.Update: vfxGraph is null!");
                return;
            }

            if (graphicsBuffer == null)
            {
                Debug.LogError("[VFX] VFXManager.Update: graphicsBuffer is null!");
                return;
            }

            vfxGraph.playRate = deltaTimeMultiplier;

            if (!GraphIsInitialized)
            {
                vfxGraph.SetGraphicsBuffer(requestsBufferId, graphicsBuffer);
                GraphIsInitialized = true;
            }

            bool isValid = graphicsBuffer.IsValid();
            if (!isValid)
            {
                return;
            }

            if (uploadDataToGraphs)
            {
                int requestsToProcess = math.min(RequestsCount.Value, Requests.Length);

                for (int i = 0; i < requestsToProcess; i++)
                {
                    graphicsBuffer.SetData(Requests, i, 0, 1);
                    vfxGraph.SetInt(requestsCountId, 1);
                    vfxGraph.SendEvent(spawnBatchId);
                }
            }

            RequestsCount.Value = 0;
        }

        public void AddRequest(T request)
        {
            if (RequestsCount.Value >= Requests.Length)
            {
                return;
            }

            Requests[RequestsCount.Value] = request;
            RequestsCount.Value++;
        }
    }
}