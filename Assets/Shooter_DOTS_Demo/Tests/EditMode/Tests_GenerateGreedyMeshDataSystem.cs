using NUnit.Framework;
using Shooter_DOTS_Demo.Code.Authoring;
using Shooter_DOTS_Demo.Code.Gameplay.Map.Components;
using Shooter_DOTS_Demo.Code.Gameplay.Map.Systems;
using Shooter_DOTS_Demo.Code.Infrastructure.States.GameStates.Components.StateTag;
using Unity.Entities;

namespace Shooter_DOTS_Demo.Tests.EditMode
{
    public class Tests_GenerateGreedyMeshDataSystem
    {
        private World _world;
        private EntityManager _em;
        private SystemHandle _systemHandle;

        [SetUp]
        public void Setup()
        {
            _world = new World("TestWorld", WorldFlags.Simulation);
            _em = _world.EntityManager;

            _systemHandle = _world.CreateSystem<GreedyVoxelsMergeToMeshDataSystem>();
            SimulationSystemGroup simGroup = _world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            simGroup.AddSystemToUpdateList(_systemHandle);
        }

        [TearDown]
        public void TearDown()
        {
            _world.Dispose();
        }

        [Test]
        public void GenerateGreedyMeshDataSystem_OnUpdate_ShouldGenerateMeshRects()
        {
            // --- Create required entities ---

            // Required for system to run
            _em.CreateEntity(typeof(GamePrepareStateTagComponent));
            _em.CreateEntity(typeof(EntitiesReferencesComponent));

            // Create chunk entity
            Entity chunkEntity = _em.CreateEntity();
            int size = 2;

            // Add buffers first before using them
            _em.AddBuffer<MeshBoxSpawnDataBufferElement>(chunkEntity);
            DynamicBuffer<VoxelBufferElement> voxelBuffer = _em.AddBuffer<VoxelBufferElement>(chunkEntity);

            // Initialize buffer with correct size (size^3 = 8 for size=2)
            int totalVoxels = size * size * size;
            voxelBuffer.ResizeUninitialized(totalVoxels);

            // Fill buffer according to indexing formula: index = x + y * size + z * size * size
            // For size=2: (0,0,0)=0, (1,0,0)=1, (0,1,0)=2, (1,1,0)=3, (0,0,1)=4, (1,0,1)=5, (0,1,1)=6, (1,1,1)=7
            voxelBuffer[0] = new VoxelBufferElement { VoxelType = 1 }; // index 0 = (0,0,0)
            voxelBuffer[1] = new VoxelBufferElement { VoxelType = 1 }; // index 1 = (1,0,0)
            voxelBuffer[2] = new VoxelBufferElement { VoxelType = 2 }; // index 2 = (0,1,0)
            voxelBuffer[3] = new VoxelBufferElement { VoxelType = 2}; // index 3 = (1,1,0)
            voxelBuffer[4] = new VoxelBufferElement { VoxelType = 2 }; // index 4 = (0,0,1)
            voxelBuffer[5] = new VoxelBufferElement { VoxelType = 2 }; // index 5 = (1,0,1)
            voxelBuffer[6] = new VoxelBufferElement { VoxelType = 2 }; // index 6 = (0,1,1)
            voxelBuffer[7] = new VoxelBufferElement { VoxelType = 2 }; // index 7 = (1,1,1)

            // Add chunk component
            _em.AddComponentData(chunkEntity, new ChunkComponent
            {
                ChunkSize = size,
                //MeshRectList = new NativeList<MeshRectData>(Allocator.Persistent)
            });
            
            _em.AddComponent<ChunkDirtyEnableTagComponent>(chunkEntity);
            _em.SetComponentEnabled<ChunkDirtyEnableTagComponent>(chunkEntity, true);

            // --- Run system ---
            _world.Update();

            // --- Read results ---
            //ChunkComponent chunkData = _em.GetComponentData<ChunkComponent>(chunkEntity);


            DynamicBuffer<MeshBoxSpawnDataBufferElement> meshBoxBuffer = _em.GetBuffer<MeshBoxSpawnDataBufferElement>(chunkEntity);
            Assert.IsTrue(meshBoxBuffer.IsCreated, "MeshRectList must be created");
            Assert.AreEqual(3, meshBoxBuffer.Length, "Should produce exactly 3 rectangles");
/*
            // --- Check first rect ---
            MeshRectData r1 = chunkData.MeshRectList[0];
            Assert.AreEqual(1, r1.VoxelType);
            Assert.AreEqual(new int3(0, 0, 0), r1.StartPosition);
            Assert.AreEqual(new int2(2, 2), r1.Size);

            // --- Check second rect ---
            MeshRectData r2 = chunkData.MeshRectList[1];
            Assert.AreEqual(2, r2.VoxelType);
            Assert.AreEqual(new int3(2, 2, 0), r2.StartPosition);
            Assert.AreEqual(new int2(2, 2), r2.Size);
            */

            // Cleanup
            meshBoxBuffer.Clear();
            //voxelBuffer.Clear();
        }
    }
}