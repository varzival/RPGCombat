using RPG.Core;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Health targetHealth;

    [SerializeField]
    float speed;

    float damage = 0;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        Vector3 aimLocation = targetHealth.transform.position;
        if (targetHealth.gameObject.TryGetComponent(out CapsuleCollider targetCollider))
        {
            aimLocation += Vector3.up * targetCollider.height / 2;
        }
        transform.LookAt(aimLocation);
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
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
            if (health == targetHealth)
            {
                targetHealth.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
