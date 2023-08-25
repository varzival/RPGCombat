using System.Collections;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField]
        Weapon weapon;

        [SerializeField]
        float respawnTime = 5f;

        bool hidden = false;

        private void OnTriggerEnter(Collider other)
        {
            if (hidden)
                return;
            if (other.gameObject.tag == "Player")
            {
                other.gameObject.GetComponent<Fighter>().EquipWeapon(weapon);
                StartCoroutine(HideForSeconds(respawnTime));
            }
        }

        private IEnumerator HideForSeconds(float seconds)
        {
            hidden = true;
            toggleChildrenActive(false);
            yield return new WaitForSeconds(seconds);
            toggleChildrenActive(true);
            hidden = false;
        }

        private void toggleChildrenActive(bool active)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(active);
            }
        }
    }
}
