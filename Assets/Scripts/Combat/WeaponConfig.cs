using RPG.Stats;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "RPG Combat/Create Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
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
        Weapon weaponPrefab;

        [SerializeField]
        bool isRightHanded = true;

        [SerializeField]
        AnimatorOverrideController weaponOverride;

        [SerializeField]
        Projectile projectile;

        const string weaponName = "Weapon";

        public bool HasProjectile
        {
            get { return projectile != null; }
        }

        public Weapon Spawn(
            Transform rightHandTransform,
            Transform leftHandTransform,
            Animator characterAnimator
        )
        {
            foreach (Transform hand in new Transform[] { rightHandTransform, leftHandTransform })
            {
                Transform oldWeapon = hand.Find(weaponName);
                if (oldWeapon)
                {
                    oldWeapon.name = "to be destroyed";
                    Destroy(oldWeapon.gameObject);
                }
            }

			GameObject newWeaponGameObject = null;
            if (
                weaponPrefab != null
                && (rightHandTransform != null || !isRightHanded)
                && (leftHandTransform != null || isRightHanded)
            )
            {
                newWeaponGameObject = Instantiate(
                    weaponPrefab.gameObject,
                    isRightHanded ? rightHandTransform : leftHandTransform
                );
                newWeaponGameObject.name = weaponName;
            }

            if (weaponOverride != null)
            {
                characterAnimator.runtimeAnimatorController = weaponOverride;
            }
            else
            {
                // Reset to parent controller if current animator controller is an override
                AnimatorOverrideController controller =
                    characterAnimator.runtimeAnimatorController as AnimatorOverrideController;
                if (controller != null)
                {
                    characterAnimator.runtimeAnimatorController =
                        controller.runtimeAnimatorController;
                }
            }

			if (newWeaponGameObject != null)
				return newWeaponGameObject.GetComponent<Weapon>();
			return null;
        }

        public void LaunchProjectile(
            Transform rightHandTransform,
            Transform leftHandTransform,
            Health target,
            float projectileDamage,
            GameObject instigator
        )
        {
            if (projectile == null)
                return;
            Projectile projectileInstance = Instantiate(
                projectile,
                isRightHanded ? rightHandTransform.position : leftHandTransform.position,
                Quaternion.identity
            );
            projectileInstance.SetTarget(target, projectileDamage, instigator);
        }
    }
}
