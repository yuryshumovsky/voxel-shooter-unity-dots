using System.Runtime.InteropServices;
using Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.UI.UIControllers;
using Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.UI.View;
using Shooter_DOTS_Demo.Code.Gameplay.VFX;
using Shooter_DOTS_Demo.Code.Infrastructure.States.GameStates;
using UnityEngine;
using UnityEngine.VFX;
using Zenject;

namespace Shooter_DOTS_Demo.Code.Gameplay.Installers
{
    public class GameplayInstaller : MonoInstaller<GameplayInstaller>
    {
        [SerializeField] private AbilitiesPanelView _abilitiesPanelView;
        [SerializeField] private VisualEffect _explosionVisualEffect;

        public override void InstallBindings()
        {
            Container.BindInstance(_abilitiesPanelView);
            Container.BindInterfacesTo<AbilitiesPanelUIController>().AsSingle().Lazy();

            InitializeVFX();
        }

        private void InitializeVFX()
        {
            VFXReferences.ExplosionsGraph = _explosionVisualEffect;
            VFXReferences.ExplosionsRequestsBuffer = new GraphicsBuffer(
                GraphicsBuffer.Target.Structured,
                GameConstants.ExplosionsCapacity,
                Marshal.SizeOf(typeof(VFXExplosionRequest))
            );
        }
    }
}