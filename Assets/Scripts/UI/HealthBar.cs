using RPG.Stats;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField]
        Image foregroundImage;

        [SerializeField]
        GameObject backgroundImage;

        [SerializeField]
        Health health;

        void OnEnable()
        {
            health.HealthChanged += ChangeValue;
        }

        void Start()
        {
            EnableHealthbar(health.GetHealthFraction());
        }

        public void ChangeValue(float fraction, float value, float maxValue)
        {
            foregroundImage.rectTransform.localScale = new Vector3(
                fraction,
                foregroundImage.rectTransform.localScale.y,
                foregroundImage.rectTransform.localScale.z
            );

            EnableHealthbar(fraction);
        }

        private void EnableHealthbar(float fraction)
        {
            backgroundImage.SetActive(
                !(Mathf.Approximately(fraction, 1) || Mathf.Approximately(fraction, 0))
            );
        }
    }
}
