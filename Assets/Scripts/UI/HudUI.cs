using RPG.Combat;
using RPG.SceneManagement;
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
        BaseStats playerBaseStats;
        SavingWrapper savingWrapper;

        Label playerHealthValue;
        Label enemyHealthValue;
        Label playerHealthPercentage;
        Label enemyHealthPercentage;
        Label playerExperienceValue;
        Label playerLevelValue;

        VisualElement fadeContainer;

        private static string GetDisplayPercentageText(float value)
        {
            return $"{Mathf.Round(value * 100)} %";
        }

        private static string GetDisplayHealthValueText(float health, float maxHealth)
        {
            return $"({Mathf.Round(health)}/{Mathf.Round(maxHealth)})";
        }

        private void Awake()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            playerHealthValue = root.Q<Label>("PlayerHealthValue");
            enemyHealthValue = root.Q<Label>("EnemyHealthValue");
            playerHealthPercentage = root.Q<Label>("PlayerHealthPercentage");
            enemyHealthPercentage = root.Q<Label>("EnemyHealthPercentage");

            playerExperienceValue = root.Q<Label>("PlayerXPValue");
            playerLevelValue = root.Q<Label>("PlayerLevelValue");

            fadeContainer = root.Q<VisualElement>("FadeContainer");
        }

        private void OnEnable()
        {
            // Assign after Fader has been spawned in Awake in PersistentObjectSpawner
            GameObject player = GameObject.FindWithTag("Player");
            playerHealth = player.GetComponent<Health>();
            playerFighter = player.GetComponent<Fighter>();
            playerExperience = player.GetComponent<Experience>();
            playerBaseStats = player.GetComponent<BaseStats>();
            savingWrapper = FindObjectOfType<SavingWrapper>();
            FindObjectOfType<Fader>().FadedTo += FadeToAlpha;

            playerHealth.HealthChanged += SetPlayerHealthText;
            playerFighter.TargetChanged += SetEnemyTarget;
            playerExperience.XPChanged += SetPlayerXPText;
            playerExperience.LevelChanged += SetPlayerLevelText;
            savingWrapper.SceneLoaded += InitializeDisplay;
        }

        private void InitializeDisplay()
        {
            UnsetEnemyHealthText();
            SetPlayerHealthText(
                playerHealth.GetHealthFraction(),
                playerHealth.Value,
                playerHealth.MaxValue
            );
            SetPlayerXPText(playerExperience.ExperiencePoints);
            Debug.Break();
            SetPlayerLevelText(playerBaseStats.Level);
        }

        private void Start()
        {
            InitializeDisplay();
        }

        private void FadeToAlpha(float alpha)
        {
            fadeContainer.style.backgroundColor = new StyleColor(new Color(0, 0, 0, alpha));
        }

        private void SetPlayerLevelText(int value)
        {
            playerLevelValue.text = value + "";
        }

        private void SetPlayerXPText(float value)
        {
            playerExperienceValue.text = Mathf.RoundToInt(value) + "";
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
