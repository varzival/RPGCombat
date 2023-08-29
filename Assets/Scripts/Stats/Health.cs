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
        float health = 100f;
        float maxHealth = 0f;
        bool dead = false;

        public bool IsDead
        {
            get { return dead; }
        }

        public event Action<float> HealthChanged;

        public float GetHealthFraction()
        {
            return health / maxHealth;
        }

        private void Start()
        {
            health = GetComponent<BaseStats>().Health;
            maxHealth = health;
            print($"Health {health} {maxHealth} {health / maxHealth}");
            HealthChanged?.Invoke(GetHealthFraction());
        }

        public void TakeDamage(float damage)
        {
            health -= damage;
            if (health <= 0)
            {
                health = 0;
                if (!dead)
                {
                    Die();
                    GetComponent<ActionScheduler>().CancelCurrentAction();
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

            if (health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            dead = true;
            GetComponent<Animator>().SetTrigger("Die");
        }
    }
}
