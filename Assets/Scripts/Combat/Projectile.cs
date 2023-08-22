using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    Transform target;

    [SerializeField]
    float speed;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        Vector3 aimLocation = target.position;
        if (target.TryGetComponent(out CapsuleCollider targetCollider))
        {
            aimLocation += Vector3.up * targetCollider.height / 2;
        }
        transform.LookAt(aimLocation);
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
    }
}
