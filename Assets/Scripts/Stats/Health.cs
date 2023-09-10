using RPG.Saving;
using RPG.Core;
using UnityEngine;
using System;
using GameDevTV.Utils;
using UnityEngine.Events;

namespace RPG.Stats
{
    [
        RequireComponent(typeof(Animator)),
        RequireComponent(typeof(ActionScheduler)),
        RequireComponent(typeof(BaseStats))
    ]
    public class Health : MonoBehaviour, ISaveable
    {
        LazyValue<float> health;
        LazyValue<float> maxHealth;

        public float Value
        {
            get { return health.value; }
        }

        public float MaxValue
        {
            get { return maxHealth.value; }
        }

        bool dead = false;

        BaseStats baseStats;

        public bool IsDead
        {
            get { return dead; }
        }

        // Health Fraction, Health, maxHealth
        public event Action<float, float, float> HealthChanged;

        public UnityEvent<float> takeDamage;

        [SerializeField]
        UnityEvent die;

        [SerializeField]
        UnityEvent heal;

        public float GetHealthFraction()
        {
            return health.value / maxHealth.value;
        }

        private void Awake()
        {
            baseStats = GetComponent<BaseStats>();
            health = new LazyValue<float>(() => baseStats.Health);
            maxHealth = new LazyValue<float>(() => baseStats.Health);
        }

        private void OnEnable()
        {
            if (TryGetComponent(out Experience experience))
            {
                experience.LevelChanged += (int level) =>
                {
                    float oldHealth = health.value;
                    float oldMaxHealth = maxHealth.value;
                    maxHealth.value = baseStats.Health;
                    health.value = Mathf.Ceil(oldHealth / oldMaxHealth * maxHealth.value);
                    //print($"Health levelled to {health.value}/{maxHealth.value}");
                    HealthChanged?.Invoke(GetHealthFraction(), health.value, maxHealth.value);
                };
            }
        }

        private void Start()
        {
            HealthChanged?.Invoke(GetHealthFraction(), health.value, maxHealth.value);
        }

        public void TakeDamage(float damage, GameObject instigator)
        {
            health.value -= damage;
            if (health.value <= 0)
            {
                health.value = 0;
                if (!dead)
                {
                    Die();
                    GetComponent<ActionScheduler>().CancelCurrentAction();
                    if (
                        instigator.TryGetComponent(out Experience instigatorExperience)
                        && TryGetComponent(out BaseStats baseStats)
                    )
                    {
                        instigatorExperience.AwardXP(baseStats.ExperienceReward);
                    }
                }
            }
            HealthChanged?.Invoke(GetHealthFraction(), health.value, maxHealth.value);
            takeDamage?.Invoke(damage);
            Debug.Log($"health after hit: {health}");
        }

        public void Heal(float healthToRestore)
        {
            health.value += healthToRestore;
            if (health.value > maxHealth.value)
                health.value = maxHealth.value;

            HealthChanged?.Invoke(GetHealthFraction(), health.value, maxHealth.value);
            heal?.Invoke();
        }

        public object CaptureState()
        {
            return new Tuple<float, float>(health.value, maxHealth.value);
        }

        public void RestoreState(object state)
        {
            Tuple<float, float> values = (Tuple<float, float>)state;
            health.value = values.Item1;
            maxHealth.value = values.Item2;
            //print($"Health restored to {health.value}/{maxHealth.value}");
            HealthChanged?.Invoke(GetHealthFraction(), health.value, maxHealth.value);

            if (health.value <= 0)
            {
                Die(false);
            }
            else
            {
                if (dead)
                {
                    GetComponent<Animator>().SetTrigger("Resurrect");
                }
                dead = false;
            }
        }

        private void Die(bool fireUnityEvent = true)
        {
            dead = true;
            GetComponent<Animator>().SetTrigger("Die");
            if (fireUnityEvent)
                die?.Invoke();
        }
    }
}
