using UnityEngine;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour
    {
        [SerializeField]
        float experiencePoints = 0f;

        public void AwardXP(float value)
        {
            experiencePoints += value;
        }
    }
}
