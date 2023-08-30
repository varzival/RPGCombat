using UnityEngine;
using System.Linq;

namespace RPG.Stats
{
    [CreateAssetMenu(
        fileName = "Progression",
        menuName = "RPG Stats/Create Progression",
        order = 0
    )]
    public class Progression : ScriptableObject
    {
        [SerializeField]
        ProgressionCharacterByClass[] progressionCharacterByClass;

        [System.Serializable]
        class ProgressionCharacterByClass
        {
            [SerializeField]
            CharacterClass characterClass;

            public CharacterClass CharacterClass
            {
                get { return characterClass; }
            }

            [System.Serializable]
            class ProgressionStat
            {
                public Stats stat;
                public float[] levels;
            }

            [SerializeField]
            ProgressionStat[] stats;

            public float GetStatByLevel(Stats stat, int level)
            {
                return stats.Single((s) => s.stat == stat).levels[level - 1];
            }
        }

        public float GetStatByLevel(CharacterClass characterClass, Stats stat, int level)
        {
            ProgressionCharacterByClass pcbc = progressionCharacterByClass.Single(
                (pcbc) => pcbc.CharacterClass == characterClass
            );
            return pcbc.GetStatByLevel(stat, level);
        }
    }
}
