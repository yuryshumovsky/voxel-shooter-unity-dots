using Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using RigidTransform = Unity.Mathematics.RigidTransform;
using RaycastHit = Unity.Physics.RaycastHit;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Utilities
{
    public static class WeaponUtilities
    {
        public static bool GetClosestValidWeaponRaycastHit(
            in NativeList<RaycastHit> hits,
            in DynamicBuffer<WeaponShotIgnoredEntityBufferData> ignoredEntities,
            out RaycastHit closestValidHit)
        {
            closestValidHit = default;
            closestValidHit.Fraction = float.MaxValue;
            
            for (int j = 0; j < hits.Length; j++)
            {
                RaycastHit tmpHit = hits[j];

                if (tmpHit.Fraction < closestValidHit.Fraction)
                {
                    bool entityValid = true;
                    for (int k = 0; k < ignoredEntities.Length; k++)
                    {
                        if (tmpHit.Entity == ignoredEntities[k].Entity)
                        {
                            entityValid = false;
                            break;
                        }
                    }

                    if (entityValid)
                    {
                        closestValidHit = tmpHit;
                    }
                }
            }

            return closestValidHit.Entity != Entity.Null;
        }
        public static RigidTransform GetShotSimulationOrigin(
            Entity shotOrigin,
            ref ComponentLookup<LocalTransform> localTransformLookup,
            ref ComponentLookup<Parent> parentLookup,
            ref ComponentLookup<PostTransformMatrix> postTransformMatrixLookup)
        {
            if (!localTransformLookup.TryGetComponent(shotOrigin, out LocalTransform localTransform))
            {
                return RigidTransform.identity;
            }

            float3 position = localTransform.Position;
            quaternion rotation = localTransform.Rotation;

            if (parentLookup.TryGetComponent(shotOrigin, out Parent parent))
            {
                RigidTransform parentTransform = GetShotSimulationOrigin(
                    parent.Value,
                    ref localTransformLookup,
                    ref parentLookup,
                    ref postTransformMatrixLookup);
                
                position = math.transform(parentTransform, position);
                rotation = math.mul(parentTransform.rot, rotation);
            }

            return new RigidTransform(rotation, position);
        }

        public static void CalculateIndividualRaycastShot(
            float3 startPosition,
            float3 direction,
            float maxDistance,
            in CollisionWorld collisionWorld,
            ref NativeList<RaycastHit> hits,
            in DynamicBuffer<WeaponShotIgnoredEntityBufferData> ignoredEntities,
            out bool hitFound,
            out float hitDistance,
            out float3 hitNormal,
            out Entity hitEntity,
            out float3 shotEndPoint)
        {
            hitFound = false;
            hitDistance = 0f;
            hitNormal = float3.zero;
            hitEntity = Entity.Null;
            shotEndPoint = startPosition + direction * maxDistance;

            hits.Clear();
            
            RaycastInput input = new RaycastInput
            {
                Start = startPosition,
                End = shotEndPoint,
                Filter = new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = ~0u,
                    GroupIndex = 0
                }
            };

            collisionWorld.CastRay(input, ref hits);

            if (hits.Length == 0)
            {
                return;
            }

            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                Entity entity = collisionWorld.Bodies[hit.RigidBodyIndex].Entity;

                bool shouldIgnore = false;
                for (int j = 0; j < ignoredEntities.Length; j++)
                {
                    if (ignoredEntities[j].Entity == entity)
                    {
                        shouldIgnore = true;
                        break;
                    }
                }

                if (!shouldIgnore)
                {
                    hitFound = true;
                    hitDistance = hit.Fraction * maxDistance;
                    hitNormal = hit.SurfaceNormal;
                    hitEntity = entity;
                    shotEndPoint = startPosition + direction * hitDistance;
                    return;
                }
            }
        }
    }
}

