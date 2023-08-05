using UnityEngine;
using RPG.Core;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour
    {
        public bool CanBeAttacked()
        {
            return !GetComponent<Health>().IsDead;
        }
    }
}
