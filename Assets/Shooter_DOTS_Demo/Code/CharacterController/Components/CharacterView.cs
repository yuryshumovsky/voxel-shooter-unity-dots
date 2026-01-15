using System;
using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.CharacterController.Components
{
    [Serializable]
    public struct CharacterView : IComponentData
    {
        public Entity CharacterEntity;
    }
}