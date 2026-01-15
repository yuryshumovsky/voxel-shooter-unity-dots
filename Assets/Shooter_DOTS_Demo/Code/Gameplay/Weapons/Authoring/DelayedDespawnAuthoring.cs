using Shooter_DOTS_Demo.Code.Misc;
using Unity.Entities;
using UnityEngine;

namespace Shooter_DOTS_Demo.Code.Gameplay.Weapons.Authoring
{
    public class DelayedDespawnAuthoring : MonoBehaviour
    {
        [SerializeField] public uint DespawnTicks = 1;
        [SerializeField] public bool active;

        public class Baker : Baker<DelayedDespawnAuthoring>
        {
            public override void Bake(DelayedDespawnAuthoring authoring)
            {
                var entity = GetEntity(authoring);
                AddComponent(entity, new DelayedDespawnComponent
                {
                    DespawnTicks = authoring.DespawnTicks,
                    Ticks = 0
                });

                SetComponentEnabled<DelayedDespawnComponent>(entity, authoring.active);
            }
        }
    }
}