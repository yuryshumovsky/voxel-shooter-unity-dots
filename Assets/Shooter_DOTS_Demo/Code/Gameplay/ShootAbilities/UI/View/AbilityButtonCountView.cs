using TMPro;
using UnityEngine;

namespace Shooter_DOTS_Demo.Code.Gameplay.ShootAbilities.UI.View
{
    public class AbilityButtonCountView : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetUnlimitCount()
        {
            SetValue("∞");
        }

        public void SetValue(string value)
        {
            text.text = value;
        }
    }
}