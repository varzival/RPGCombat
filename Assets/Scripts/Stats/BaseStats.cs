using UnityEngine;

namespace RPG.Stats
{
    [RequireComponent(typeof(Experience))]
    public class BaseStats : MonoBehaviour
    {
        [SerializeField]
        CharacterClass characterClass;

        [SerializeField]
        Progression progression;

        [SerializeField]
        public int Level
        {
            get
            {
                int totalLevels = progression.GetStatLevels(
                    characterClass,
                    Stats.ExperienceToLevelUp
                );
                if (totalLevels == -1)
                    return 1;

                float currentXP = GetComponent<Experience>().ExperiencePoints;

                float GetXPToLevel(int currentLevel)
                {
                    return progression.GetStatByLevel(
                        characterClass,
                        Stats.ExperienceToLevelUp,
                        currentLevel
                    );
                }

                for (int level = 1; level <= totalLevels; level++)
                {
                    float xpToLevelUp = GetXPToLevel(level);
                    if (currentXP < xpToLevelUp)
                        return level;
                }
                return totalLevels + 1;
            }
        }

        public float Health
        {
            get { return progression.GetStatByLevel(characterClass, Stats.Health, Level); }
        }

        public float ExperienceReward
        {
            get
            {
                return progression.GetStatByLevel(characterClass, Stats.ExperienceReward, Level);
            }
        }

        public float DamageModifier
        {
            get { return progression.GetStatByLevel(characterClass, Stats.DamageModifier, Level); }
        }
    }
}
