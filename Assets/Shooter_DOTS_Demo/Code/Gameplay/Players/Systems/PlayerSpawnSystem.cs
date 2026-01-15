using Shooter_DOTS_Demo.Code.Authoring;
using Shooter_DOTS_Demo.Code.CharacterController.Components;
using Shooter_DOTS_Demo.Code.Gameplay.Common;
using Shooter_DOTS_Demo.Code.Gameplay.Players.Components;
using Shooter_DOTS_Demo.Code.Gameplay.Weapons.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Shooter_DOTS_Demo.Code.Gameplay.Players.Systems
{
    public partial struct SpawnMyPlayerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EntitiesReferencesComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityManager entityManager = state.EntityManager;

            EntityCommandBuffer entityCommandBuffer = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            EntitiesReferencesComponent entitiesReferences = SystemAPI.GetSingleton<EntitiesReferencesComponent>();

            foreach (var (spawnRequestComponent, spawnRequestEntity) in SystemAPI
                         .Query<RefRO<SpawnPlayerRequest>>()
                         .WithEntityAccess()
                    )
            {
                DynamicBuffer<GameResourcesWeapon> weaponPrefabs = SystemAPI.GetSingletonBuffer<GameResourcesWeapon>();

                if (weaponPrefabs.IsEmpty)
                {
                    Debug.LogError("Weapon prefabs buffer is empty");
                    entityCommandBuffer.DestroyEntity(spawnRequestEntity);
                    continue;
                }

                if (!entityManager.Exists(entitiesReferences.PlayerGhost))
                {
                    Debug.LogError($"PlayerGhost entity is invalid. Index: {entitiesReferences.PlayerGhost.Index}, Version: {entitiesReferences.PlayerGhost.Version}");
                    entityCommandBuffer.DestroyEntity(spawnRequestEntity);
                    continue;
                }

                if (!entityManager.Exists(entitiesReferences.CharacterGhost))
                {
                    Debug.LogError($"CharacterGhost entity is invalid. Index: {entitiesReferences.CharacterGhost.Index}, Version: {entitiesReferences.CharacterGhost.Version}");
                    entityCommandBuffer.DestroyEntity(spawnRequestEntity);
                    continue;
                }

                float3 spawnPosition = new float3(42f, 10f, 27f);

                Entity playerEntity = entityCommandBuffer.Instantiate(entitiesReferences.PlayerGhost);
                Entity characterEntity = entityCommandBuffer.Instantiate(entitiesReferences.CharacterGhost);

                if (entityManager.HasComponent<PlayerComponent>(entitiesReferences.PlayerGhost))
                {
                    PlayerComponent fppComponent =
                        entityManager.GetComponentData<PlayerComponent>(entitiesReferences.PlayerGhost);
                    fppComponent.ControlledCharacter = characterEntity;

                    entityCommandBuffer.SetComponent(playerEntity, fppComponent);
                }
                else
                {
                    Debug.LogError($"Error: Can't get {typeof(PlayerComponent)}");
                }

                entityCommandBuffer.SetComponent(characterEntity, LocalTransform.FromPosition(spawnPosition));
                entityCommandBuffer.DestroyEntity(spawnRequestEntity);
                entityCommandBuffer.SetName(playerEntity, (FixedString64Bytes)$"player_{playerEntity.Index}");
                entityCommandBuffer.SetName(characterEntity, (FixedString64Bytes)$"character_{characterEntity.Index}");

                Entity randomWeaponPrefab = weaponPrefabs[0].WeaponPrefab;

                if (!entityManager.Exists(randomWeaponPrefab))
                {
                    Debug.LogError($"Weapon prefab entity is invalid. Index: {randomWeaponPrefab.Index}, Version: {randomWeaponPrefab.Version}");
                    entityCommandBuffer.DestroyEntity(spawnRequestEntity);
                    continue;
                }

                Entity weaponEntity = entityCommandBuffer.Instantiate(randomWeaponPrefab);
                entityCommandBuffer.SetName(weaponEntity, (FixedString64Bytes)$"weapon_{weaponEntity.Index}");
                
                entityCommandBuffer.SetComponent(
                    characterEntity,
                    new ActiveWeaponComponent()
                    {
                        CurrentWeaponEntity = weaponEntity, 
                        PreviousWeaponEntity = Entity.Null
                    }
                );

                if (entityManager.HasComponent<CharacterInitializedComponent>(entitiesReferences.CharacterGhost))
                {
                    entityCommandBuffer.SetComponentEnabled<CharacterInitializedComponent>(characterEntity, true);
                }
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}