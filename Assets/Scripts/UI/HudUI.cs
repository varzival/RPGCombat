using RPG.CharacterControl;
using RPG.Combat;
using RPG.Stats;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.UI
{
    public class HudUI : MonoBehaviour, IUIElements
    {
        Health playerHealth;
        Fighter playerFighter;
        Experience playerExperience;
        Health enemyHealth;

        VisualElement root;
        VisualElement[] UIVisualElements; // these elements block the cursor on hover
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
            root = GetComponent<UIDocument>().rootVisualElement;

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

        private void Start()
        {
            // These Elements will count as UI and block player interaction
            UIVisualElements = new VisualElement[]
            {
                root.Q<VisualElement>("HealthContainer"),
                root.Q<VisualElement>("ExperienceContainer")
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

        public CursorType GetCursorType()
        {
            return CursorType.None;
        }

        public bool HandleMouseInUI()
        {
            if (root == null || UIVisualElements == null || UIVisualElements.Length == 0)
                return false;

            foreach (VisualElement v in UIVisualElements)
            {
                Vector2 screenPosition = new Vector2(
                    Input.mousePosition.x,
                    root.resolvedStyle.height - Input.mousePosition.y
                );

                Vector2 panelPosition = RuntimePanelUtils.ScreenToPanel(root.panel, screenPosition);
                if (v.worldBound.Contains(panelPosition))
                    return true;
            }
            return false;
        }
    }
}
