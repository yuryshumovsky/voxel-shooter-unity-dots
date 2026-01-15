using System.Collections.Generic;
using Shooter_DOTS_Demo.Code.Gameplay.Common;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Shooter_DOTS_Demo.Code.Authoring
{
    public class EntitiesReferencesAuthoring : MonoBehaviour
    {
        [SerializeField] public Transform brickPrefabGameObject;
        [SerializeField] public Transform simpleShellPrefabGameObject;
        [SerializeField] public GameObject BalloonPopExplosion;
        
        [SerializeField] public List<Color> brickColors;
        [SerializeField] public List<GameObject> WeaponGhosts = new List<GameObject>();
        [Header("Ghost Prefabs")] public GameObject PlayerGhost;
        public GameObject CharacterGhost;
        
        public class Baker : Unity.Entities.Baker<EntitiesReferencesAuthoring>
        {
            public override void Bake(EntitiesReferencesAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            
                BlobAssetReference<ColorsBlob> colorsBlob = CreateColorsBlob(authoring.brickColors);

                AddComponent(entity, new EntitiesReferencesComponent
                {
                    brickPrefabEntity = GetEntity(authoring.brickPrefabGameObject, TransformUsageFlags.Dynamic),
                    simpleShellPrefabEntity = GetEntity(authoring.simpleShellPrefabGameObject, TransformUsageFlags.Dynamic),
                    PlayerGhost = GetEntity(authoring.PlayerGhost, TransformUsageFlags.Dynamic),
                    CharacterGhost = GetEntity(authoring.CharacterGhost, TransformUsageFlags.Dynamic),
                    BalloonPopExplosion = GetEntity(authoring.BalloonPopExplosion, TransformUsageFlags.Dynamic),
                    colorsBlob = colorsBlob
                });
                
                DynamicBuffer<GameResourcesWeapon> weaponsBuffer = AddBuffer<GameResourcesWeapon>(entity);
                for (int i = 0; i < authoring.WeaponGhosts.Count; i++)
                {
                    weaponsBuffer.Add(new GameResourcesWeapon
                    {
                        WeaponPrefab = GetEntity(authoring.WeaponGhosts[i], TransformUsageFlags.Dynamic),
                    });
                }
            }

            private BlobAssetReference<ColorsBlob> CreateColorsBlob(List<Color> colors)
            {
                using (var builder = new BlobBuilder(Allocator.Temp))
                {
                    ref ColorsBlob colorBlob = ref builder.ConstructRoot<ColorsBlob>();
                
                    BlobBuilderArray<float4> colorArray = builder.Allocate(ref colorBlob.colorsArray, colors.Count);
                
                    for (int i = 0; i < colors.Count; i++)
                    {
                        Color color = colors[i];
                        colorArray[i] = new float4(color.r, color.g, color.b, color.a);
                    }
                
                    return builder.CreateBlobAssetReference<ColorsBlob>(Allocator.Persistent);
                }
            }
        }
    }

    public struct ColorsBlob
    {
        public BlobArray<float4> colorsArray;
    }

    public struct EntitiesReferencesComponent : IComponentData
    {
        public Entity brickPrefabEntity;
        public Entity simpleShellPrefabEntity;
       
        public BlobAssetReference<ColorsBlob> colorsBlob;
        public Entity PlayerGhost;
        public Entity CharacterGhost;
        public Entity BalloonPopExplosion;
    }
}