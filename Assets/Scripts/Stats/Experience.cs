using System;
using RPG.Saving;
using UnityEngine;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField]
        float experiencePoints = 0f;

        public event Action<float> XPChanged;

        public void AwardXP(float value)
        {
            experiencePoints += value;
            XPChanged?.Invoke(experiencePoints);
        }

        object ISaveable.CaptureState()
        {
            return experiencePoints;
        }

        void ISaveable.RestoreState(object state)
        {
            experiencePoints = (float)state;
            XPChanged?.Invoke(experiencePoints);
        }
    }
}
