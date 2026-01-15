using Shooter_DOTS_Demo.Code.CharacterController.Components;
using Unity.Entities;
using UnityEngine;

namespace Shooter_DOTS_Demo.Code.CharacterController.Authoring
{
    [DisallowMultipleComponent]
    public class PlayerAuthoring : MonoBehaviour
    {
        public GameObject ControlledCharacter;
        public float LookInputSensitivity = 0.2f;

        public class Baker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new PlayerComponent
                {
                    ControlledCharacter = GetEntity(authoring.ControlledCharacter, TransformUsageFlags.Dynamic),
                    LookInputSensitivity = authoring.LookInputSensitivity,
                });
                AddComponent<PlayerInputsComponent>(entity);
            }
        }
    }
}
