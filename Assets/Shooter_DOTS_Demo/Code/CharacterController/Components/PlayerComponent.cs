using System;
using Unity.Entities;
using UnityEngine.Serialization;

namespace Shooter_DOTS_Demo.Code.CharacterController.Components
{
    [Serializable]
    public struct PlayerComponent : IComponentData
    {
        public Entity ControlledCharacter;
        [FormerlySerializedAs("LookRotationSpeed")] public float LookInputSensitivity;
    }
}