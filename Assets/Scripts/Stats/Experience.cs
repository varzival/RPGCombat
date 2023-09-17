using System;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Stats
{
    [RequireComponent(typeof(BaseStats))]
    public class Experience : MonoBehaviour, ISaveable, IJSONSaveable
    {
        float experiencePoints = 0f;

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

        [SerializeField]
        UnityEvent LevelUp;

        public void AwardXP(float value)
        {
            int oldLevel = baseStats.Level;
            experiencePoints += value;
            CheckNewLevelAndLevelUp(oldLevel);
            XPChanged?.Invoke(experiencePoints);
        }

        private void CheckNewLevelAndLevelUp(int oldLevel, bool fireUnityEvent = true)
        {
            int newLevel = baseStats.Level;
            if (newLevel > oldLevel)
            {
                Debug.Log($"Level of {gameObject} changed to {baseStats.Level}");
                LevelChanged?.Invoke(newLevel);
                if (fireUnityEvent)
                    LevelUp?.Invoke();
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
            CheckNewLevelAndLevelUp(oldLevel, false);
            XPChanged?.Invoke(experiencePoints);
            Debug.Log($"Level {gameObject} restored to {baseStats.Level}");
        }

        public JToken CaptureStateAsJToken()
        {
            return JToken.FromObject(experiencePoints);
        }

        public void RestoreStateFromJToken(JToken state)
        {
            int oldLevel = baseStats.Level;
            experiencePoints = state.ToObject<float>();
            CheckNewLevelAndLevelUp(oldLevel, false);
            XPChanged?.Invoke(experiencePoints);
            Debug.Log($"Level {gameObject} restored to {baseStats.Level}");
        }
    }
}
