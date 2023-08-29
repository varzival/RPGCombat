using RPG.Stats;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.UI
{
    public class HudUI : MonoBehaviour
    {
        Health playerHealth;

        private static string GetDisplayPercentageText(float value)
        {
            return Mathf.Round(value * 100) + " %";
        }

        private void OnEnable()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            playerHealth = GameObject.FindWithTag("Player").GetComponent<Health>();
            Label healthValue = root.Q<Label>("HealthValue");

            playerHealth.HealthChanged += (float value) =>
            {
                healthValue.text = GetDisplayPercentageText(value);
            };
        }
    }
}
