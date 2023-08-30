using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 10)]
        [SerializeField]
        int level = 1;

        [SerializeField]
        CharacterClass characterClass;

        [SerializeField]
        Progression progression;

        public float Health
        {
            get { return progression.GetStatByLevel(characterClass, Stats.Health, level); }
        }

        public float ExperienceReward
        {
            get
            {
                return progression.GetStatByLevel(characterClass, Stats.ExperienceReward, level);
            }
        }
    }
}
