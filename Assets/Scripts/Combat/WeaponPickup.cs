using System.Collections;
using RPG.CharacterControl;
using RPG.Characters;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField]
        WeaponConfig weapon;

        [SerializeField]
        CursorType cursorType;

        [SerializeField]
        float respawnTime = 5f;

        bool hidden = false;

        public bool HandleRaycast(PlayerController playerController, Vector3 raycastHitPoint)
        {
            if (hidden)
                return false;
            if (Input.GetMouseButton(0))
            {
                playerController.GetComponent<Mover>().StartMoveAction(raycastHitPoint);
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return cursorType;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hidden)
                return;
            if (other.gameObject.tag == "Player")
            {
                Pickup(other.gameObject.GetComponent<Fighter>());
            }
        }

        private void Pickup(Fighter fighter)
        {
            fighter.EquipWeapon(weapon);
            StartCoroutine(HideForSeconds(respawnTime));
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
