using Shooter_DOTS_Demo.Code.Gameplay.Map.Configs;
using Shooter_DOTS_Demo.Code.Gameplay.Players.Configs;
using Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.Configs;
using UnityEngine;
using Zenject;

namespace Shooter_DOTS_Demo.Code.Gameplay.Common
{
    [CreateAssetMenu(fileName = "GameConfigsInstaller", menuName = "Installers/GameConfigsInstaller")]
    public class GameConfigsInstaller : ScriptableObjectInstaller<GameConfigsInstaller>
    {
        [SerializeField] private FigureConfig _figureConfig;
        [SerializeField] private PlayersConfig _playersConfig;
        [SerializeField] private ShootAbilitiesConfig _shootAbilitiesConfig;

        public override void InstallBindings()
        {
            Container.BindInstance(_figureConfig);
            Container.BindInstance(_playersConfig);
            Container.BindInstance(_shootAbilitiesConfig);
        }
    }
}