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

            private ProgressionStat MatchingStat(Stats stat)
            {
                return stats.SingleOrDefault((s) => s.stat == stat);
            }

            public float GetStatByLevel(Stats stat, int level)
            {
                ProgressionStat matchingStat = MatchingStat(stat);
                if (matchingStat != null)
                    return matchingStat.levels[level - 1];
                else
                    return -1;
            }

            public int GetStatLevels(Stats stat)
            {
                ProgressionStat matchingStat = MatchingStat(stat);
                if (matchingStat != null)
                    return matchingStat.levels.Length;
                else
                    return -1;
            }
        }

        private ProgressionCharacterByClass GetPCBCFromClass(CharacterClass characterClass)
        {
            return progressionCharacterByClass.SingleOrDefault(
                (pcbc) => pcbc.CharacterClass == characterClass
            );
        }

        public float GetStatByLevel(CharacterClass characterClass, Stats stat, int level)
        {
            return GetPCBCFromClass(characterClass).GetStatByLevel(stat, level);
        }

        public int GetStatLevels(CharacterClass characterClass, Stats stat)
        {
            return GetPCBCFromClass(characterClass).GetStatLevels(stat);
        }
    }
}
