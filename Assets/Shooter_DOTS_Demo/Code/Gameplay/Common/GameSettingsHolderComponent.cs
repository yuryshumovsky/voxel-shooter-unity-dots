using Shooter_DOTS_Demo.Code.Gameplay.Map.Configs;
using Shooter_DOTS_Demo.Code.Gameplay.Players.Configs;
using Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.Configs;
using Unity.Entities;

namespace Shooter_DOTS_Demo.Code.Gameplay.Common
{
    public struct GameSettingsHolderComponent : IComponentData
    {
        public UnityObjectRef<FigureConfig> figureConfig;
        public UnityObjectRef<PlayersConfig> playersConfig;
        public UnityObjectRef<ShootAbilitiesConfig> shootAbilitiesConfig;
    }
}