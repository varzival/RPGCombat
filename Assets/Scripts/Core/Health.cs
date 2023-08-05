using RPG.Saving;
using UnityEngine;

namespace RPG.Core
{
    [RequireComponent(typeof(Animator)), RequireComponent(typeof(ActionScheduler))]
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField]
        float health = 100f;
        bool dead = false;

        public bool IsDead
        {
            get { return dead; }
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
