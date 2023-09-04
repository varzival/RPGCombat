using UnityEngine;
using RPG.Stats;
using RPG.CharacterControl;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public bool CanBeAttacked()
        {
            return !GetComponent<Health>().IsDead;
        }

        public bool HandleRaycast(PlayerController playerController, Vector3 raycastHitPoint)
        {
            if (!CanBeAttacked())
                return false;
            if (Input.GetMouseButton(0))
            {
                playerController.GetComponent<Fighter>().Attack(GetComponent<Health>());
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }
    }
}
