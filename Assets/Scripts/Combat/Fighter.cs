using RPG.Characters;
using RPG.Core;
using RPG.Stats;
using RPG.Saving;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace RPG.Combat
{
    [
        RequireComponent(typeof(Mover)),
        RequireComponent(typeof(Animator)),
        RequireComponent(typeof(ActionScheduler)),
        RequireComponent(typeof(BaseStats))
    ]
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
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

        public float WeaponDamage
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

        BaseStats baseStats;

        Weapon currentWeapon;

        Health target;
        float timeSinceLastAttack = Mathf.Infinity;

        public Health Target
        {
            get { return target; }
        }

        public event Action<Health> TargetChanged;

        private void Awake()
        {
            baseStats = GetComponent<BaseStats>();
        }

        private void Start()
        {
            if (currentWeapon == null)
                EquipWeapon(defaultWeapon);
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (target != null && !target.IsDead)
            {
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
            return Vector3.Distance(target.transform.position, transform.position);
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
            TargetChanged?.Invoke(target);
            GetComponent<ActionScheduler>().StartAction(this);
        }

        public void Cancel()
        {
            this.target = null;
            TargetChanged?.Invoke(null);
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
                if (currentWeapon.HasProjectile)
                {
                    currentWeapon.LaunchProjectile(
                        rightHandTransform,
                        leftHandTransform,
                        target,
                        baseStats.Damage,
                        gameObject
                    );
                }
                else
                    target.TakeDamage(baseStats.Damage, gameObject);
            }
            else
                Debug.Log("target not in range");
        }

        // Animation Event
        public void Shoot()
        {
            Hit();
        }

        public object CaptureState()
        {
            return currentWeapon.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            if (weaponName != null && weaponName != "")
                EquipWeapon(Resources.Load<Weapon>(weaponName));
        }

        public IEnumerable<float> GetAdditiveProvider(Stats.Stats stat)
        {
            if (stat == Stats.Stats.Damage)
                return new float[] { WeaponDamage };
            return new float[] { };
        }

        public IEnumerable<float> GetMultiplicativeProvider(Stats.Stats stat)
        {
            return new float[] { };
        }
    }
}
