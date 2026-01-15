using System.Collections.Generic;
using NUnit.Framework;
using Shooter_DOTS_Demo.Code.Gameplay.Map.Components;
using Shooter_DOTS_Demo.Code.Gameplay.Map.Components.Singletons;
using Shooter_DOTS_Demo.Code.Gameplay.Map.Configs;
using Shooter_DOTS_Demo.Code.Gameplay.Map.Services;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Shooter_DOTS_Demo.Tests.EditMode
{
    public class Tests_MapVoxelsAndChunksInitializeService
    {
        private World _world;
        private EntityManager _em;
        private MapVoxelsAndChunksInitializeService _service;
        private World _previousWorld;

        [SetUp]
        public void Setup()
        {
            // Сохраняем предыдущий world, если он есть
            _previousWorld = World.DefaultGameObjectInjectionWorld;

            // Создаем тестовый world
            _world = new World("TestWorld", WorldFlags.Simulation);
            _em = _world.EntityManager;

            // Устанавливаем тестовый world как DefaultGameObjectInjectionWorld
            // так как сервис использует World.DefaultGameObjectInjectionWorld
            World.DefaultGameObjectInjectionWorld = _world;

            // Создаем мок FigureConfig
            FigureConfig figureConfig = ScriptableObject.CreateInstance<FigureConfig>();

            // Создаем экземпляр сервиса
            _service = new MapVoxelsAndChunksInitializeService();
        }

        [TearDown]
        public void TearDown()
        {
            // Очищаем NativeHashMap если он был создан
            using (var query = _em.CreateEntityQuery(typeof(ChunksMapSingletonComponent)))
            {
                if (!query.IsEmpty)
                {
                    var chunksHolder = query.GetSingleton<ChunksMapSingletonComponent>();
                    if (chunksHolder.ChunksMap.IsCreated)
                    {
                        chunksHolder.ChunksMap.Dispose();
                    }
                }
            }

            // Восстанавливаем предыдущий world
            if (_previousWorld != null)
            {
                World.DefaultGameObjectInjectionWorld = _previousWorld;
            }

            _world.Dispose();
        }

        [Test]
        public void InitializeChunksFromVoxels_WithMoreThanTwoChunks_ShouldCreateCorrectNumberOfChunks()
        {
            // Arrange
            // Создаем воксели размером 40x40x40 (больше чем 32x32x32 = 2 чанка)
            // Для 40x40x40: max = 39, chunksCount = ceil(39/16) = 3 по каждой оси
            // Итого должно быть 3 * 3 * 3 = 27 чанков
            const int voxelSize = 17;
            Dictionary<int3, byte> loadedVoxels = new Dictionary<int3, byte>();

            // Заполняем воксели (можно заполнить все или только часть для теста)
            for (int x = 0; x < voxelSize; x++)
            {
                for (int y = 0; y < voxelSize; y++)
                {
                    for (int z = 0; z < voxelSize; z++)
                    {
                        // Используем простой паттерн для типа вокселя
                        byte voxelType = (byte)((x + y + z) % 5);
                        loadedVoxels[new int3(x, y, z)] = voxelType;
                    }
                }
            }

            // Act
            _service.InitializeChunksFromVoxels(loadedVoxels, 8);

            // Assert
            // Проверяем, что создан singleton с WorldChunksHolderComponent
            ChunksMapSingletonComponent chunksMapSingleton;

            using (EntityQuery query = _em.CreateEntityQuery(typeof(ChunksMapSingletonComponent)))
            {
                //Test 0 : WorldChunksHolderComponent exist
                Assert.IsFalse(
                    query.IsEmpty,
                    "WorldChunksHolderComponent singleton should be created"
                );
                chunksMapSingleton = query.GetSingleton<ChunksMapSingletonComponent>();
            }

            //Test 1: Chunks.IsCreated
            Assert.IsTrue(
                chunksMapSingleton.ChunksMap.IsCreated,
                "Chunks NativeHashMap should be created"
            );

            const int chunkSize = 8;

            // Проверяем количество чанков
            // Для 40x40x40: ceil(39/16) = 3 по каждой оси, итого 27 чанков
            int expectedChunksPerAxis = (int)math.ceil((float)voxelSize / (float)chunkSize);

            int expectedTotalChunks = expectedChunksPerAxis * expectedChunksPerAxis * expectedChunksPerAxis; // 27

            //
            Assert.AreEqual(
                expectedTotalChunks,
                chunksMapSingleton.ChunksMap.Count,
                $"Should create {expectedTotalChunks} chunks (3x3x3) for 40x40x40 voxels"
            );

            // Проверяем каждый чанк

            const int expectedVoxelBufferSize = chunkSize * chunkSize * chunkSize; // 4096

            int checkedChunks = 0;
            foreach (KVPair<int3, Entity> kvp in chunksMapSingleton.ChunksMap)
            {
                int3 chunkCoord = kvp.Key;
                Entity chunkEntity = kvp.Value;

                // Проверяем, что entity существует
                Assert.IsTrue(
                    _em.Exists(chunkEntity),
                    $"Chunk entity at {chunkCoord} should exist"
                );

                // Проверяем наличие ChunkComponent
                Assert.IsTrue(
                    _em.HasComponent<ChunkComponent>(chunkEntity),
                    $"Chunk at {chunkCoord} should have ChunkComponent"
                );

                ChunkComponent chunkComponent = _em.GetComponentData<ChunkComponent>(chunkEntity);
                Assert.AreEqual(
                    chunkSize,
                    chunkComponent.ChunkSize,
                    $"Chunk at {chunkCoord} should have ChunkSize = {chunkSize}"
                );

                Assert.AreEqual(
                    new int3(chunkCoord.x * chunkSize, chunkCoord.y * chunkSize, chunkCoord.z * chunkSize),
                    chunkComponent.ChunkPosition,
                    $"Chunk at {chunkCoord} should have correct ChunkCoord"
                );

                // Проверяем наличие VoxelBufferData
                Assert.IsTrue(
                    _em.HasComponent<VoxelBufferElement>(chunkEntity),
                    $"Chunk at {chunkCoord} should have VoxelBufferData buffer"
                );

                DynamicBuffer<VoxelBufferElement> voxelBuffer = _em.GetBuffer<VoxelBufferElement>(chunkEntity);
                Assert.IsTrue(
                    voxelBuffer.IsCreated,
                    $"VoxelBufferData at {chunkCoord} should be created"
                );
                Assert.AreEqual(
                    expectedVoxelBufferSize,
                    voxelBuffer.Length,
                    $"Chunk at {chunkCoord} should have {expectedVoxelBufferSize} VoxelBufferData elements (16^3)"
                );

                checkedChunks++;
            }

            Assert.AreEqual(
                expectedTotalChunks,
                checkedChunks,
                "Should check all chunks"
            );
        }
    }
}