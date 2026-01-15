using System;
using Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.Components;
using Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.Configs;
using Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.Enums;
using Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.UI.View;
using Shooter_DOTS_Demo.Code.Utils;
using Unity.Collections;
using Unity.Entities;
using Zenject;

namespace Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.UI.UIControllers
{
    public class AbilitiesPanelUIController : IInitializable, ITickable, ILateTickable, IDisposable
    {
        private readonly AbilitiesPanelView _panelView;
        private readonly ShootAbilitiesConfig _shootAbilitiesConfig;
        private EntityManager _entityManager;
        private EntityQuery _query;

        public AbilitiesPanelUIController(AbilitiesPanelView panelView, ShootAbilitiesConfig shootAbilitiesConfig)
        {
            _panelView = panelView;
            _shootAbilitiesConfig = shootAbilitiesConfig;
        }

        public void Initialize()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _query = _entityManager.CreateEntityQuery(typeof(AbilityViewDataComponent));

            _panelView.OnChooseAbility += ActionPanel_OnChooseAbility;
        }

        private void ActionPanel_OnChooseAbility(ShootAbilityType type)
        {
            EntityUtils
                .CreateEntity()
                .AddComponent(new ChooseAbilityRequestComponent() { type = type })
                ;
        }

        public void LateTick()
        {
            UpdatePanelState();
        }

        public void Tick()
        {
        }

        private void UpdatePanelState()
        {
            using (NativeArray<AbilityViewDataComponent> abilityDisplayComponents = _query.ToComponentDataArray<AbilityViewDataComponent>(Allocator.Temp))
            {
                foreach (AbilityViewDataComponent viewData in abilityDisplayComponents)
            {
                if (!_panelView.HasAbility(viewData.type))
                {
                    _panelView.AddAbility(
                        viewData.type,
                        _shootAbilitiesConfig.GetConfigByType(viewData.type).buttonIcon
                    );
                }

                    _panelView.UpdateState(
                        viewData.type,
                        viewData.progress,
                        viewData.enableToUse,
                        viewData.selected
                    );
                }
            }
        }

        public void Dispose()
        {
            _panelView.OnChooseAbility -= ActionPanel_OnChooseAbility;
        }
    }
}