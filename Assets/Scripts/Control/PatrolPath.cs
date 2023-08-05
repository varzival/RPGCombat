using System.Collections.Generic;
using UnityEngine;

namespace RPG.CharacterControl
{
    public class PatrolPath : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Gizmos.DrawSphere(transform.GetChild(i).position, 0.3f);
                Gizmos.DrawLine(
                    transform.GetChild(i).position,
                    transform.GetChild((i + 1) % transform.childCount).position
                );
            }
        }

        public Transform GetWaypoint(int i)
        {
            return transform.GetChild(i % transform.childCount);
        }
    }
}
