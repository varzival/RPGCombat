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
        public float WeaponRange
        {
            get
            {
                if (currentWeapon != null)
                    return currentWeapon.WeaponRange;
                return 0;
            }
        }

        public float TimeBetweenAttacks
        {
            get
            {
                if (currentWeapon != null)
                    return currentWeapon.TimeBetweenAttacks;
                return 0;
            }
        }

        public float Damage
        {
            get
            {
                if (currentWeapon != null)
                    return currentWeapon.Damage;
                return 0;
            }
        }

        [SerializeField]
        Weapon defaultWeapon;

        [SerializeField]
        Transform rightHandTransform;

        [SerializeField]
        Transform leftHandTransform;

        [SerializeField]
        Weapon currentWeapon;

        Health target;
        float timeSinceLastAttack = Mathf.Infinity;

        public Health Target
        {
            get { return target; }
        }

        private void Start()
        {
            EquipWeapon(defaultWeapon);
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

        public void EquipWeapon(Weapon weapon)
        {
            if (weapon == null)
                return;
            weapon.Spawn(rightHandTransform, leftHandTransform, GetComponent<Animator>());
            currentWeapon = weapon;
        }

        private bool TargetInRange()
        {
            return DistanceToTarget() <= WeaponRange;
        }

        private float DistanceToTarget()
        {
            return (target.transform.position - transform.position).magnitude;
        }

        private void AttackBehaviour()
        {
            GetComponent<Mover>().Cancel();
            if (timeSinceLastAttack > TimeBetweenAttacks)
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
                target.TakeDamage(Damage);
            }
            else
                Debug.Log("target not in range");
        }
    }
}
