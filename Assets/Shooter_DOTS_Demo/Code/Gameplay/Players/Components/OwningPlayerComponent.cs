using System;
using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Gameplay.Players.Components
{
    [Serializable]
    public struct OwningPlayerComponent : IComponentData
    {
        public Entity Entity;
    }
    
    public struct CharacterInitializedComponent : IComponentData, IEnableableComponent { }

}