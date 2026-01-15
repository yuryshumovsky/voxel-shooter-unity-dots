using System;
using System.Collections.Generic;
using Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.Enums;
using Shooter_DOTS_Demo.Code.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.UI.View
{
    public class AbilitiesPanelView : MonoBehaviour
    {
        public event Action<ShootAbilityType> OnChooseAbility;

        [SerializeField] private AbilityActionButtonView _itemPrefab;

        private Dictionary<ShootAbilityType, AbilityActionButtonView> _actionButtonByAbilityType = new();


        private void Start()
        {
            transform.gameObject.DestroyAllChildren();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void AddAbility(ShootAbilityType shootAbilityType, Sprite icon)
        {
            if (!_actionButtonByAbilityType.ContainsKey(shootAbilityType))
            {
                AbilityActionButtonView abilityActionButtonView = Object.Instantiate(_itemPrefab, transform);
                abilityActionButtonView.Setup(shootAbilityType, icon);
                abilityActionButtonView.OnClick += AbilityActionButton_OnClick;

                _actionButtonByAbilityType.Add(shootAbilityType, abilityActionButtonView);
            }
            else
            {
                Debug.LogError($"Error: panel already exist '{shootAbilityType}'");
            }
        }

        private void AbilityActionButton_OnClick(ShootAbilityType type)
        {
            OnChooseAbility?.Invoke(type);
        }

        public void RemoveAbility()
        {
        }

        public void UpdateState(ShootAbilityType type, float progress, bool enableToUse, bool selected)
        {
            if (_actionButtonByAbilityType.TryGetValue(type, out AbilityActionButtonView view))
            {
                view.UpdateState(progress, enableToUse, selected);
            }
            else
            {
                Debug.LogError($"Error: Try to update {type} but it doesn't exist in dictionary");
            }
        }

        public bool HasAbility(ShootAbilityType type)
        {
            return _actionButtonByAbilityType.ContainsKey(type);
        }
    }
}