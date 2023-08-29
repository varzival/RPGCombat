using RPG.Combat;
using RPG.Stats;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.UI
{
    public class HudUI : MonoBehaviour
    {
        Health playerHealth;
        Fighter playerFighter;
        Health enemyHealth;

        Label playerHealthValue;
        Label enemyHealthValue;

        private static string GetDisplayPercentageText(float value)
        {
            return $"{Mathf.Round(value * 100)} %";
        }

        private void OnEnable()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            playerHealth = GameObject.FindWithTag("Player").GetComponent<Health>();
            playerFighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();

            playerHealthValue = root.Q<Label>("PlayerHealthValue");
            enemyHealthValue = root.Q<Label>("EnemyHealthValue");
            UnsetEnemyHealthText();

            playerHealth.HealthChanged += (float value) =>
            {
                playerHealthValue.text = GetDisplayPercentageText(value);
            };
            playerFighter.TargetChanged += SetEnemyTarget;
        }

        private void SetEnemyTarget(Health target)
        {
            if (enemyHealth != null)
                enemyHealth.HealthChanged -= SetEnemyHealthText;

            enemyHealth = target;
            if (enemyHealth != null)
            {
                SetEnemyHealthText(target.GetHealthFraction());
                enemyHealth.HealthChanged += SetEnemyHealthText;
            }
            else
            {
                UnsetEnemyHealthText();
            }
        }

        private void SetEnemyHealthText(float value)
        {
            enemyHealthValue.text = GetDisplayPercentageText(value);
        }

        private void UnsetEnemyHealthText()
        {
            enemyHealthValue.text = "N/A";
        }
    }
}
