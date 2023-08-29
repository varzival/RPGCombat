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

            [SerializeField]
            float[] health;

            public float GetHealthByLevel(int level)
            {
                return health[level - 1];
            }

            [SerializeField]
            float[] damage;

            public float GetDamageByLevel(int level)
            {
                return damage[level - 1];
            }
        }

        public float GetHealth(CharacterClass characterClass, int level)
        {
            ProgressionCharacterByClass pcbc = progressionCharacterByClass.Single(
                (pcbc) => pcbc.CharacterClass == characterClass
            );
            return pcbc.GetHealthByLevel(level);
        }
    }
}
