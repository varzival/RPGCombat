using System;
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
        Label playerHealthPercentage;
        Label enemyHealthPercentage;
        Label playerExperienceValue;
        Label playerLevelValue;

        private static string GetDisplayPercentageText(float value)
        {
            return $"{Mathf.Round(value * 100)} %";
        }

        private static string GetDisplayHealthValueText(float health, float maxHealth)
        {
            return $"({Mathf.Round(health)}/{Mathf.Round(maxHealth)})";
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
            playerHealthPercentage = root.Q<Label>("PlayerHealthPercentage");
            enemyHealthPercentage = root.Q<Label>("EnemyHealthPercentage");

            playerExperienceValue = root.Q<Label>("PlayerXPValue");
            playerLevelValue = root.Q<Label>("PlayerLevelValue");
            UnsetEnemyHealthText();

            playerHealth.HealthChanged += SetPlayerHealthText;
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
                SetEnemyHealthText(target.GetHealthFraction(), target.Value, target.MaxValue);
                enemyHealth.HealthChanged += SetEnemyHealthText;
            }
            else
            {
                UnsetEnemyHealthText();
            }
        }

        private void SetEnemyHealthText(float fraction, float health, float maxHealth)
        {
            enemyHealthPercentage.text = GetDisplayPercentageText(fraction);
            enemyHealthValue.text = GetDisplayHealthValueText(health, maxHealth);
        }

        private void SetPlayerHealthText(float fraction, float health, float maxHealth)
        {
            playerHealthPercentage.text = GetDisplayPercentageText(fraction);
            playerHealthValue.text = GetDisplayHealthValueText(health, maxHealth);
        }

        private void UnsetEnemyHealthText()
        {
            enemyHealthPercentage.text = "N/A";
            enemyHealthValue.text = "N/A";
        }
    }
}
