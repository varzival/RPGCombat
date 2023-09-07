using UnityEngine;

namespace RPG.Core
{
    public class FaceCamera : MonoBehaviour
    {
        // LateUpdate instead of Update fixes rotational jitter that can happen when moving quickly
        void LateUpdate()
        {
            transform.forward = Camera.main.transform.forward;
        }
    }
}
