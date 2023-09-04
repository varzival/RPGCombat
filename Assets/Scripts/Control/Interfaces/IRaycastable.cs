using UnityEngine;

namespace RPG.CharacterControl
{
    public interface IRaycastable
    {
        CursorType GetCursorType();
        bool HandleRaycast(PlayerController playerController, Vector3 raycastHitPoint);
    }
}
