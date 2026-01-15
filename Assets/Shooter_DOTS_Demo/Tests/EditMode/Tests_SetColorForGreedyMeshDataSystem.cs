using NUnit.Framework;
using Shooter_DOTS_Demo.Code.Authoring;
using Shooter_DOTS_Demo.Code.Gameplay.Map.Components;
using Shooter_DOTS_Demo.Code.Gameplay.Map.Systems;
using Shooter_DOTS_Demo.Code.Infrastructure.States.GameStates.Components.StateTag;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Shooter_DOTS_Demo.Tests.EditMode
{
    public class Tests_SetColorForGreedyMeshDataSystem
    {
        private World _world;
        private EntityManager _em;
        private SystemHandle _systemHandle;
        private BlobAssetReference<ColorsBlob> _colorsBlob;

        [SetUp]
        public void Setup()
        {
            _world = new World("TestWorld", WorldFlags.Simulation);
            _em = _world.EntityManager;

            _systemHandle = _world.CreateSystem<SetColorForGreedyMeshDataSystem>();
            SimulationSystemGroup simGroup = _world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            simGroup.AddSystemToUpdateList(_systemHandle);

            // Создаем BlobAsset с цветами для теста
            _colorsBlob = CreateColorsBlob(new[]
            {
                new Color(1f, 0f, 0f, 1f), // Type 0 - Red
                new Color(0f, 1f, 0f, 1f), // Type 1 - Green
                new Color(0f, 0f, 1f, 1f), // Type 2 - Blue
                new Color(1f, 1f, 0f, 1f), // Type 3 - Yellow
            });
        }

        [TearDown]
        public void TearDown()
        {
            // Освобождаем BlobAsset
            if (_colorsBlob.IsCreated)
            {
                _colorsBlob.Dispose();
            }

            _world.Dispose();
        }

        private BlobAssetReference<ColorsBlob> CreateColorsBlob(Color[] colors)
        {
            using (var builder = new BlobBuilder(Allocator.Temp))
            {
                ref ColorsBlob colorBlob = ref builder.ConstructRoot<ColorsBlob>();

                BlobBuilderArray<float4> colorArray = builder.Allocate(ref colorBlob.colorsArray, colors.Length);

                for (int i = 0; i < colors.Length; i++)
                {
                    Color color = colors[i];
                    colorArray[i] = new float4(color.r, color.g, color.b, color.a);
                }

                return builder.CreateBlobAssetReference<ColorsBlob>(Allocator.Persistent);
            }
        }

        [Test]
        public void SetColorForGreedyMeshDataSystem_OnUpdate_WithMultipleBoxesSameType_ShouldSetSameColor()
        {
            // Arrange
            _em.CreateEntity(typeof(GamePrepareStateTagComponent));

            Entity referencesEntity = _em.CreateEntity();
            _em.AddComponentData(referencesEntity, new EntitiesReferencesComponent
            {
                colorsBlob = _colorsBlob,
                brickPrefabEntity = Entity.Null,
                simpleShellPrefabEntity = Entity.Null
            });

            Entity chunkEntity = _em.CreateEntity();
            _em.AddComponent<ChunkDirtyEnableTagComponent>(chunkEntity);
            _em.SetComponentEnabled<ChunkDirtyEnableTagComponent>(chunkEntity, true);
            
            DynamicBuffer<MeshBoxSpawnDataBufferElement> meshBoxBuffer = _em.AddBuffer<MeshBoxSpawnDataBufferElement>(chunkEntity);


            // Добавляем несколько mesh boxes с одинаковым типом
            meshBoxBuffer.Add(new MeshBoxSpawnDataBufferElement
            {
                VoxelType = 1,
                PositionInChunk = new int3(0, 0, 0),
                Size = new int3(1, 1, 1),
                Color = float4.zero,
                ChunkEntity = chunkEntity
            });

            meshBoxBuffer.Add(new MeshBoxSpawnDataBufferElement
            {
                VoxelType = 1,
                PositionInChunk = new int3(1, 0, 0),
                Size = new int3(1, 1, 1),
                Color = float4.zero,
                ChunkEntity = chunkEntity
            });

            // Act
            _world.Update();

            // Assert
            meshBoxBuffer = _em.GetBuffer<MeshBoxSpawnDataBufferElement>(chunkEntity);
            float4 expectedColor = new float4(0f, 1f, 0f, 1f); // Green
            const float tolerance = 0.0001f;

            AssertColorsEqual(
                expectedColor,
                meshBoxBuffer[0].Color,
                tolerance,
                "First box with VoxelType 1 should have green color"
            );

            AssertColorsEqual(
                expectedColor,
                meshBoxBuffer[1].Color,
                tolerance,
                "Second box with VoxelType 1 should have green color"
            );
        }

        private void AssertColorsEqual(float4 expected, float4 actual, float tolerance, string message)
        {
            Assert.AreEqual(expected.x, actual.x, tolerance, $"{message} - Red component mismatch");
            Assert.AreEqual(expected.y, actual.y, tolerance, $"{message} - Green component mismatch");
            Assert.AreEqual(expected.z, actual.z, tolerance, $"{message} - Blue component mismatch");
            Assert.AreEqual(expected.w, actual.w, tolerance, $"{message} - Alpha component mismatch");
        }
    }
}