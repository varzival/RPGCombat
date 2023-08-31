using RPG.Saving;
using RPG.Core;
using UnityEngine;
using System;

namespace RPG.Stats
{
    [
        RequireComponent(typeof(Animator)),
        RequireComponent(typeof(ActionScheduler)),
        RequireComponent(typeof(BaseStats))
    ]
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField]
        float health = -1;
        float maxHealth = 0f;
        bool dead = false;

        BaseStats baseStats;

        public bool IsDead
        {
            get { return dead; }
        }

        public event Action<float> HealthChanged;

        public float GetHealthFraction()
        {
            return health / maxHealth;
        }

        private void Awake()
        {
            baseStats = GetComponent<BaseStats>();
        }

        private void Start()
        {
            if (health == -1)
            {
                health = baseStats.Health;
            }

            maxHealth = baseStats.Health;
            HealthChanged?.Invoke(GetHealthFraction());

            if (TryGetComponent(out Experience experience))
            {
                experience.LevelChanged += (int level) =>
                {
                    float oldHealth = health;
                    float oldMaxHealth = maxHealth;
                    maxHealth = baseStats.Health;
                    health = Mathf.Ceil(oldHealth / oldMaxHealth * maxHealth);
                    HealthChanged?.Invoke(GetHealthFraction());
                };
            }
        }

        public void TakeDamage(float damage, GameObject instigator)
        {
            health -= damage;
            if (health <= 0)
            {
                health = 0;
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
            HealthChanged?.Invoke(GetHealthFraction());
            Debug.Log($"health after hit: {health}");
        }

        public object CaptureState()
        {
            return health;
        }

        public void RestoreState(object state)
        {
            health = (float)state;
            HealthChanged?.Invoke(GetHealthFraction());

            if (health <= 0)
            {
                Die();
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

        private void Die()
        {
            dead = true;
            GetComponent<Animator>().SetTrigger("Die");
        }
    }
}
