using UnityEngine;
using RPG.Characters;
using RPG.Combat;
using RPG.Core;

namespace RPG.CharacterControl
{
    [
        RequireComponent(typeof(Mover)),
        RequireComponent(typeof(Fighter)),
        RequireComponent(typeof(ActionScheduler))
    ]
    public class PlayerController : MonoBehaviour
    {
        Health health;

        private void Start()
        {
            health = GetComponent<Health>();
        }

        // Update is called once per frame
        void Update()
        {
            if (health.IsDead)
                return;

            if (InteractWithCombat())
            {
                return;
            }
            if (InteractWithMovement())
            {
                return;
            }
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("No action possible.");
            }
        }

        private bool InteractWithCombat()
        {
            RaycastHit[] raycastHits = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hit in raycastHits)
            {
                if (hit.transform.gameObject.TryGetComponent(out CombatTarget target))
                {
                    if (!target.CanBeAttacked())
                        continue;
                    if (Input.GetMouseButton(0))
                    {
                        GetComponent<Fighter>().Attack(target.GetComponent<Health>());
                    }
                    return true;
                }
            }
            return false;
        }

        private bool InteractWithMovement()
        {
            if (Physics.Raycast(GetMouseRay(), out RaycastHit raycastHit))
            {
                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(raycastHit.point);
                }
                return true;
            }

            return false;
        }

        private Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
