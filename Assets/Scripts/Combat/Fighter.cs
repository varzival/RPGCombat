using RPG.Characters;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    [
        RequireComponent(typeof(Mover)),
        RequireComponent(typeof(Animator)),
        RequireComponent(typeof(ActionScheduler))
    ]
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField]
        float weaponRange = 2f;

        [SerializeField]
        float timeBetweenAttacks = 0.5f;

        [SerializeField]
        float damage = 10f;

        Health target;
        float timeSinceLastAttack = Mathf.Infinity;

        public Health Target
        {
            get { return target; }
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (target != null && !target.IsDead)
            {
                //float distanceToTarget = Vector3.Distance(target.position, transform.position);
                if (!TargetInRange())
                    GetComponent<Mover>().MoveTo(target.transform.position);
                else
                {
                    AttackBehaviour();
                }
            }
        }

        private bool TargetInRange()
        {
            return DistanceToTarget() <= weaponRange;
        }

        private float DistanceToTarget()
        {
            return (target.transform.position - transform.position).magnitude;
        }

        private void AttackBehaviour()
        {
            GetComponent<Mover>().Cancel();
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                transform.LookAt(target.transform);
                GetComponent<Animator>().ResetTrigger("StopAttack");
                GetComponent<Animator>().SetTrigger("Attack");
                timeSinceLastAttack = 0f;
            }
        }

        public void Attack(Health target)
        {
            this.target = target;
            GetComponent<ActionScheduler>().StartAction(this);
        }

        public void Cancel()
        {
            this.target = null;
            GetComponent<Animator>().SetTrigger("StopAttack");
            GetComponent<Animator>().ResetTrigger("Attack");
        }

        // Animation Event
        public void Hit()
        {
            Debug.Log($"Hit Animation Event called at: {Time.time} with target {target}");
            if (target is null)
                return;
            if (TargetInRange())
            {
                target.TakeDamage(damage);
            }
            else
                Debug.Log("target not in range");
        }
    }
}
