using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "RPG Combat/Create Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField]
        float weaponRange = 2f;

        public float WeaponRange
        {
            get { return weaponRange; }
        }

        [SerializeField]
        float timeBetweenAttacks = 0.5f;

        public float TimeBetweenAttacks
        {
            get { return timeBetweenAttacks; }
        }

        [SerializeField]
        float damage = 10f;

        public float Damage
        {
            get { return damage; }
        }

        [SerializeField]
        GameObject weaponPrefab;

        [SerializeField]
        bool isRightHanded = true;

        [SerializeField]
        AnimatorOverrideController weaponOverride;

        [SerializeField]
        Projectile projectile;

        public bool HasProjectile
        {
            get { return projectile != null; }
        }

        public void Spawn(
            Transform rightHandTransform,
            Transform leftHandTransform,
            Animator characterAnimator
        )
        {
            if (
                weaponPrefab != null
                && (rightHandTransform != null || !isRightHanded)
                && (leftHandTransform != null || isRightHanded)
            )
            {
                Instantiate(weaponPrefab, isRightHanded ? rightHandTransform : leftHandTransform);
            }
            if (weaponOverride != null)
                characterAnimator.runtimeAnimatorController = weaponOverride;
        }

        public void LaunchProjectile(
            Transform rightHandTransform,
            Transform leftHandTransform,
            Health target
        )
        {
            if (projectile == null)
                return;
            Projectile projectileInstance = Instantiate(
                projectile,
                isRightHanded ? rightHandTransform.position : leftHandTransform.position,
                Quaternion.identity
            );
            projectileInstance.SetTarget(target, damage);
        }
    }
}
