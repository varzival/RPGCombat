using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        Health targetHealth;

        [SerializeField]
        float speed;

        float damage = 0;

        [SerializeField]
        bool homingProjectile = false;

        [SerializeField]
        float maxAliveTime = 5f;

        [SerializeField]
        GameObject impactEffect;

        float currentAliveTime = 0f;

        // Start is called before the first frame update
        void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        // Update is called once per frame
        void Update()
        {
            if (homingProjectile && !targetHealth.IsDead)
                transform.LookAt(GetAimLocation());
            transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
            currentAliveTime += Time.deltaTime;
            if (currentAliveTime >= maxAliveTime)
                Destroy(gameObject);
        }

        private Vector3 GetAimLocation()
        {
            Vector3 aimLocation = targetHealth.transform.position;
            if (targetHealth.gameObject.TryGetComponent(out CapsuleCollider targetCollider))
            {
                aimLocation += Vector3.up * targetCollider.height / 2;
            }
            return aimLocation;
        }

        public void SetTarget(Health target, float damage)
        {
            this.targetHealth = target;
            this.damage = damage;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Health health))
            {
                if (health == targetHealth && !targetHealth.IsDead)
                {
                    targetHealth.TakeDamage(damage);
                    if (impactEffect != null)
                        Instantiate(impactEffect, transform.position, transform.rotation);
                    Destroy(gameObject);
                }
            }
        }
    }
}
