using System;
using RPG.Saving;
using UnityEngine;

namespace RPG.Stats
{
    [RequireComponent(typeof(BaseStats))]
    public class Experience : MonoBehaviour, ISaveable
    {
        float experiencePoints = 0f;

        [SerializeField]
        GameObject levelUpEffect;

        BaseStats baseStats;

        public float ExperiencePoints
        {
            get { return experiencePoints; }
        }

        private void Awake()
        {
            baseStats = GetComponent<BaseStats>();
        }

        public event Action<float> XPChanged;
        public event Action<int> LevelChanged;

        public void AwardXP(float value)
        {
            int oldLevel = baseStats.Level;
            experiencePoints += value;
            CheckNewLevelAndLevelUp(oldLevel);
            XPChanged?.Invoke(experiencePoints);
        }

        private void CheckNewLevelAndLevelUp(int oldLevel)
        {
            int newLevel = baseStats.Level;
            if (newLevel > oldLevel)
            {
                Debug.Log($"Level of {gameObject} changed to {baseStats.Level}");
                LevelChanged?.Invoke(newLevel);
                Instantiate(levelUpEffect, transform);
            }
        }

        object ISaveable.CaptureState()
        {
            return experiencePoints;
        }

        void ISaveable.RestoreState(object state)
        {
            int oldLevel = baseStats.Level;
            experiencePoints = (float)state;
            CheckNewLevelAndLevelUp(oldLevel);
            XPChanged?.Invoke(experiencePoints);
            Debug.Log($"Level {gameObject} restored to {baseStats.Level}");
        }
    }
}
