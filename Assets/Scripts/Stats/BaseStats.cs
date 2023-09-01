using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor.VersionControl;
using UnityEngine;

namespace RPG.Stats
{
    [RequireComponent(typeof(Experience))]
    public class BaseStats : MonoBehaviour, IModifierProvider
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

        private float GetStatWithModifiers(Stats stat)
        {
            return GetAdditiveModifiers(stat) * GetMultiplicativeModifiers(stat);
        }

        private float GetAdditiveModifiers(Stats stat)
        {
            float value = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float additiveModifier in provider.GetAdditiveProvider(stat))
                {
                    value += additiveModifier;
                }
            }
            return value;
        }

        private float GetMultiplicativeModifiers(Stats stat)
        {
            float value = 1;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float multiplicativeModifier in provider.GetMultiplicativeProvider(stat))
                {
                    value *= multiplicativeModifier;
                }
            }
            return value;
        }

        public IEnumerable<float> GetAdditiveProvider(Stats stat)
        {
            switch (stat)
            {
                case Stats.Health:
                case Stats.ExperienceReward:
                case Stats.ExperienceToLevelUp:
                    return new float[] { GetStat(stat) };
                default:
                    return new float[] { };
            }
        }

        public IEnumerable<float> GetMultiplicativeProvider(Stats stat)
        {
            switch (stat)
            {
                case Stats.DamageModifier:
                    return new float[] { GetStat(stat) };
                case Stats.Damage:
                    return new float[] { GetStat(Stats.DamageModifier) };
                default:
                    return new float[] { };
            }
        }

        private float GetStat(Stats stat)
        {
            return progression.GetStatByLevel(characterClass, stat, Level);
        }

        public float Health
        {
            get { return GetStatWithModifiers(Stats.Health); }
        }

        public float ExperienceReward
        {
            get { return GetStatWithModifiers(Stats.ExperienceReward); }
        }

        public float Damage
        {
            get { return GetStatWithModifiers(Stats.Damage); }
        }
    }
}
