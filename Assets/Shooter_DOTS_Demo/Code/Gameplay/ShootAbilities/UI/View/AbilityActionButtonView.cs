using System;
using Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.UI.View
{
    public class AbilityActionButtonView : MonoBehaviour
    {
        [SerializeField] private Color selectedColor;
        [SerializeField] private Color readyColor;
        [SerializeField] private Color repairColor;

        [SerializeField] private Image progressImage;
        [SerializeField] private Image backgroundImage;

        [SerializeField] private AbilityButtonCountView countView;
        [SerializeField] private Button button;
        [SerializeField] private Image iconImage;
        
        
        private ShootAbilityType _shootAbilityType;

        public event Action<ShootAbilityType> OnClick;

        private void Start()
        {
            button.onClick.AddListener(ClickHandler);
        }

        public void Setup(ShootAbilityType shootAbilityType, Sprite icon)
        {
            _shootAbilityType = shootAbilityType;
            iconImage.sprite = icon;
        }

        private void ClickHandler()
        {
            OnClick.Invoke(_shootAbilityType);
        }

        public float RepairProgress
        {
            set { progressImage.fillAmount = value; }
        }

        public void UpdateState(float progress, bool enableToUse, bool selected)
        {
            if (enableToUse)
            {
                backgroundImage.color = readyColor;
                progressImage.fillAmount = 0;
            }
            else
            {
                backgroundImage.color = repairColor;
                progressImage.fillAmount = 1 - progress;
            }
        }
    }
}