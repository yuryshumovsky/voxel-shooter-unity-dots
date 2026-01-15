using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Shooter_DOTS_Demo.Code.CharacterController.Components
{
    [Serializable]
    public struct CharacterControlComponent : IComponentData
    {
        public float3 MoveVector;
        public float2 LookDegreesDelta;
        public bool Jump;
    }
}