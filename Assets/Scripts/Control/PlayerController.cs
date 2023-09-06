using UnityEngine;
using RPG.Characters;
using RPG.Combat;
using RPG.Core;
using RPG.Stats;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.AI;
using System.IO;

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

        [Serializable]
        struct CursorMapping
        {
            public CursorType cursorType;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField]
        CursorMapping[] cursorMappings;

        [SerializeField]
        GameObject[] uiElementGameObjects;

        IEnumerable<IUIElements> uiElements;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        float navMeshTolerance = 0.3f;

        [SerializeField]
        float maxNavMeshDistance = 10f;

        private void Start()
        {
            health = GetComponent<Health>();
            uiElements = uiElementGameObjects
                .Select((go) => go.GetInterfaces<IUIElements>())
                .Aggregate(
                    new List<IUIElements>(),
                    (current, next) =>
                    {
                        current.AddRange(next);
                        return current;
                    }
                );
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

            if (InteractWithComponent())
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

        private bool InteractWithComponent()
        {
            RaycastHit[] raycastHits = RaycastAllSorted();
            foreach (RaycastHit hit in raycastHits)
            {
                if (hit.transform.gameObject.TryGetComponent(out IRaycastable raycastable))
                {
                    if (raycastable.HandleRaycast(this, hit.point))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        private bool InteractWithUI()
        {
            foreach (IUIElements uel in uiElements)
            {
                if (uel.HandleMouseInUI())
                {
                    SetCursor(uel.GetCursorType());
                    return true;
                }
            }
            return false;
        }

        private bool InteractWithMovement()
        {
            if (RaycastNavMesh(out Vector3 target))
            {
                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(target);
                }
                SetCursor(CursorType.Movement);
                return true;
            }

            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            if (Physics.Raycast(GetMouseRay(), out RaycastHit raycastHit))
            {
                if (
                    NavMesh.SamplePosition(
                        raycastHit.point,
                        out NavMeshHit hit,
                        navMeshTolerance,
                        NavMesh.AllAreas
                    )
                )
                {
                    target = hit.position;
                    NavMeshPath path = new();
                    if (NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path))
                    {
                        return path.status == NavMeshPathStatus.PathComplete
                            && GetPathLength(path) <= maxNavMeshDistance;
                    }
                    return false;
                }
            }
            target = Vector3.zero;
            return false;
        }

        private float GetPathLength(NavMeshPath path)
        {
            if (path.corners?.Length < 2)
                return 0;
            Vector3 lastCorner = Vector3.zero;
            return path.corners.Aggregate(
                0f,
                (current, next) =>
                {
                    if (lastCorner == Vector3.zero)
                    {
                        lastCorner = next;
                        return current;
                    }
                    else
                    {
                        current += Vector3.Distance(lastCorner, next);
                        lastCorner = next;
                        return current;
                    }
                }
            );
        }

        private Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        private RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            IEnumerable<float> distances = hits.Select((h) => h.distance);
            Array.Sort(distances.ToArray(), hits);
            return hits;
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
