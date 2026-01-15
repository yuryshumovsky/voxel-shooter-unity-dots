using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Shooter_DOTS_Demo.Code.Utils
{
    public static class TransformHelpers
    {
        public static void ComputeWorldTransformMatrix(
            Entity entity,
            out float4x4 worldMatrix,
            ref ComponentLookup<LocalTransform> localTransformLookup,
            ref ComponentLookup<Parent> parentLookup,
            ref ComponentLookup<PostTransformMatrix> postTransformMatrixLookup)
        {
            float4x4 localMatrix = float4x4.identity;
            
            if (localTransformLookup.TryGetComponent(entity, out LocalTransform localTransform))
            {
                localMatrix = float4x4.TRS(
                    localTransform.Position,
                    localTransform.Rotation,
                    localTransform.Scale);
            }

            if (postTransformMatrixLookup.TryGetComponent(entity, out PostTransformMatrix postTransform))
            {
                localMatrix = math.mul(localMatrix, postTransform.Value);
            }

            if (parentLookup.TryGetComponent(entity, out Parent parent))
            {
                ComputeWorldTransformMatrix(
                    parent.Value,
                    out float4x4 parentMatrix,
                    ref localTransformLookup,
                    ref parentLookup,
                    ref postTransformMatrixLookup);
                worldMatrix = math.mul(parentMatrix, localMatrix);
            }
            else
            {
                worldMatrix = localMatrix;
            }
        }

        public static float3 Translation(this float4x4 matrix)
        {
            return matrix.c3.xyz;
        }
    }
}

