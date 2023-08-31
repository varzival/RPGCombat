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
        Experience playerExperience;
        Health enemyHealth;

        Label playerHealthValue;
        Label enemyHealthValue;
        Label playerExperienceValue;
        Label playerLevelValue;

        private static string GetDisplayPercentageText(float value)
        {
            return $"{Mathf.Round(value * 100)} %";
        }

        private void OnEnable()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            GameObject player = GameObject.FindWithTag("Player");
            playerHealth = player.GetComponent<Health>();
            playerFighter = player.GetComponent<Fighter>();
            playerExperience = player.GetComponent<Experience>();

            playerHealthValue = root.Q<Label>("PlayerHealthValue");
            enemyHealthValue = root.Q<Label>("EnemyHealthValue");
            playerExperienceValue = root.Q<Label>("PlayerXPValue");
            playerLevelValue = root.Q<Label>("PlayerLevelValue");
            UnsetEnemyHealthText();

            playerHealth.HealthChanged += (float value) =>
            {
                playerHealthValue.text = GetDisplayPercentageText(value);
            };
            playerFighter.TargetChanged += SetEnemyTarget;
            playerExperience.XPChanged += (float value) =>
            {
                playerExperienceValue.text = Mathf.RoundToInt(value) + "";
            };
            playerExperience.LevelChanged += (int value) =>
            {
                playerLevelValue.text = value + "";
            };
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
