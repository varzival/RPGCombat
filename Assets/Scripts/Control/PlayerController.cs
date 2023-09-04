using UnityEngine;
using RPG.Characters;
using RPG.Combat;
using RPG.Core;
using RPG.Stats;
using System;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

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

        enum CursorType
        {
            None,
            Movement,
            Combat
        }

        [Serializable]
        struct CursorMapping
        {
            public CursorType cursorType;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField]
        CursorMapping[] cursorMappings;

        VisualElement[] UIVisualElements;
        VisualElement root;

        public void SetUIVisualElements(VisualElement[] UIVisualElements, VisualElement root)
        {
            this.UIVisualElements = UIVisualElements;
            this.root = root;
        }

        private void Start()
        {
            health = GetComponent<Health>();
        }

        // Update is called once per frame
        void Update()
        {
            if (InteractWithUI())
            {
                return;
            }
            if (health.IsDead)
            {
                SetCursor(CursorType.None);
                return;
            }

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
            SetCursor(CursorType.None);
        }

        private bool InteractWithUI()
        {
            if (MouseInsideUI())
            {
                SetCursor(CursorType.None);
                return true;
            }
            return false;
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
                    SetCursor(CursorType.Combat);
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
                SetCursor(CursorType.Movement);
                return true;
            }

            return false;
        }

        private Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        private bool MouseInsideUI()
        {
            if (root == null || UIVisualElements == null || UIVisualElements.Length == 0)
                return false;

            foreach (VisualElement v in UIVisualElements)
            {
                Vector2 screenPosition = new Vector2(
                    Input.mousePosition.x,
                    root.resolvedStyle.height - Input.mousePosition.y
                );

                Vector2 panelPosition = RuntimePanelUtils.ScreenToPanel(root.panel, screenPosition);
                if (v.worldBound.Contains(panelPosition))
                    return true;
            }
            return false;
        }

        private CursorMapping GetCursorMapping(CursorType cursorType)
        {
            return cursorMappings.Single(
                (CursorMapping mapping) => mapping.cursorType == cursorType
            );
        }

        private void SetCursor(CursorType cursorType)
        {
            CursorMapping mapping = GetCursorMapping(cursorType);
            UnityEngine.Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }
    }
}
