using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    public class TraitStore : MonoBehaviour, IModifierProvider
    {
        Dictionary<Trait, int> assignedPoints = new Dictionary<Trait, int>();
        Dictionary<Trait, int> stagedPoints = new Dictionary<Trait, int>();
        Dictionary<Stat, Dictionary<Trait, float>> additiveBonusCache = new Dictionary<Stat, Dictionary<Trait, float>>();
        Dictionary<Stat, Dictionary<Trait, float>> percentageBonusCache = new Dictionary<Stat, Dictionary<Trait, float>>();

        [SerializeField] TraitBonus[] bonusConfig = new TraitBonus[0];

        private void Awake() 
        {
            additiveBonusCache = new Dictionary<Stat, Dictionary<Trait, float>>();
            percentageBonusCache = new Dictionary<Stat, Dictionary<Trait, float>>();

            foreach(var traitBonus in bonusConfig)
            {
                if(!additiveBonusCache.ContainsKey(traitBonus.stat))
                    additiveBonusCache[traitBonus.stat] = new Dictionary<Trait, float>();

                if(!percentageBonusCache.ContainsKey(traitBonus.stat))
                    percentageBonusCache[traitBonus.stat] = new Dictionary<Trait, float>();

                additiveBonusCache[traitBonus.stat][traitBonus.trait] = traitBonus.additiveTraitBonusPerPoint;
                percentageBonusCache[traitBonus.stat][traitBonus.trait] = traitBonus.percentageTraitBonusPerPoint;
            }
        }

        #region //Getting points
        public int GetPoints(Trait trait)
        {
            return assignedPoints.ContainsKey(trait) ? assignedPoints[trait] : 0;
        }

        public int GetStagedPoints(Trait trait)
        {
            return stagedPoints.ContainsKey(trait) ? stagedPoints[trait] : 0;
        }

        public int GetProposedPoints(Trait trait)
        {
            return GetPoints(trait) + GetStagedPoints(trait);
        }

        int GetTotalProposedPoints()
        {
            int total = 0;
            foreach(int points in assignedPoints.Values)
                total += points;

            foreach(int points in stagedPoints.Values)
                total += points;

            return total;
        }

        public int GetAssignablePoints()
        {
            return (int)GetComponent<BaseStats>().GetStat(Stat.TotalTraitPoints);
        }
        #endregion

        #region //Assigning points
        public void AssignPoints(Trait trait, int points)
        {
            stagedPoints[trait] = GetStagedPoints(trait) + points;
        }

        public bool CanAssignPoints(Trait trait, int points)
        {
            if(GetStagedPoints(trait) + points < 0) return false;
            if(GetUnassignedPoints() < points) return false;
            return true;
        }

        public int GetUnassignedPoints()
        {
            return GetAssignablePoints() - GetTotalProposedPoints();
        }

        public void Commit()
        {
            foreach(Trait trait in stagedPoints.Keys)
            {
                assignedPoints[trait] = GetProposedPoints(trait);
            }

            stagedPoints.Clear();
        }
        #endregion

        public IEnumerable<float> GetAdditiveModifier(Stat stat)
        {
            if(!additiveBonusCache.ContainsKey(stat)) yield break;
            foreach(Trait trait in additiveBonusCache[stat].Keys)
            {
                yield return additiveBonusCache[stat][trait] * GetPoints(trait);
            }
        }

        public IEnumerable<float> GetPercentageModifier(Stat stat)
        {
            if(!percentageBonusCache.ContainsKey(stat)) yield break;
            foreach(Trait trait in percentageBonusCache[stat].Keys)
            {
                yield return percentageBonusCache[stat][trait] * GetPoints(trait);
            }
        }

        [System.Serializable]
        class TraitBonus
        {
            public Trait trait;
            public Stat stat;
            public float additiveTraitBonusPerPoint;
            public float percentageTraitBonusPerPoint;
        }
    }
}
