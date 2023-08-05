using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        public Transform target;

        void Update()
        {
            transform.position = target.position;
        }
    }
}
