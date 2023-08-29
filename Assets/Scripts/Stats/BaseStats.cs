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
            get { return progression.GetHealth(characterClass, level); }
        }

        private float exprerienceAward = 10f;
        public float ExperienceAward
        {
            get { return exprerienceAward; }
        }
    }
}
