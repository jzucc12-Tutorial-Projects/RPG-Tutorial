using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameDevTV.Utils;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)] [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass = CharacterClass.Player;
        [SerializeField] Progression progression = null;
        [SerializeField] bool shouldUseModifiers = false;

        LazyValue<int> currentLevel;
        public event Action onLevelUp;
        Experience experience;

        private void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            currentLevel.ForceInit();
        }

        private void OnEnable()
        {
            if(experience)
                experience.onExperienceGained += UpdateLevel;
        }

        private void OnDisable()
        {
            if (experience)
                experience.onExperienceGained -= UpdateLevel;
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if(newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                print("Levelled Up!");
                onLevelUp();
            }
        }

        public int GetStat(Stat stat)
        {
            return Mathf.RoundToInt((GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1+(float)GetPercentageModifier(stat)/100));
        }

        private int GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        private int GetAdditiveModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;
            int total = 0;
            IModifierProvider[] providers = GetComponents<IModifierProvider>();
            foreach(IModifierProvider provider in providers)
            {
                foreach(int modifiers in provider.GetAdditiveModifier(stat))
                {
                    total += modifiers;
                }
            }
            return total;
        }

        private int GetPercentageModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;
            int total = 0;
            IModifierProvider[] providers = GetComponents<IModifierProvider>();
            foreach (IModifierProvider provider in providers)
            {
                foreach (int modifiers in provider.GetPercentageModifier(stat))
                {
                    total += modifiers;
                }
            }
            return total;
        }

        public int GetLevel()
        {
            return currentLevel.value;
        }

        public int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();
            if (experience == null) return startingLevel;
            float currentXP = experience.GetPoints();
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
            for(int currentLevel = 1; currentLevel <= penultimateLevel; currentLevel++)
            {
                float XPNeeded = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, currentLevel);
                if (currentXP < XPNeeded) return currentLevel;
            }
            return penultimateLevel+1;
        }
    }
}
