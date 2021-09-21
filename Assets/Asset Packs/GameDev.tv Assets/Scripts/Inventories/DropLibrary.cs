using GameDevTV.Inventories;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = "RPG/Inventory/Drop Library")]
    public class DropLibrary : ScriptableObject
    {
        [SerializeField] DropConfig[] potentialDrops = null;
        [SerializeField] float[] dropChancePercentage = null;
        [SerializeField] int[] minDrops = null;
        [SerializeField] int[] maxDrops = null;

        [System.Serializable]
        class DropConfig
        {
            public InventoryItem item = null;
            public float[] relativeChance = null;
            public int[] minNumber = null;
            public int[] maxNumber = null;
            public int GetRandomNumber(int level)
            {
                if (!item.IsStackable())
                    return 1;

                int min = GetByLevel(minNumber, level);
                int max = GetByLevel(maxNumber, level);
                return Random.Range(min, max + 1);
            }
        }

        public struct Dropped
        {
            public InventoryItem item;
            public int number;
        }

        public IEnumerable<Dropped> GetRandomDrops(int level)
        {
            if (!ShouldRandomDrop(level))
                yield break;

            for(int ii = 0; ii < GetRandomNumberOfDrops(level); ii++)
            {
                yield return GetRandomDrop(level);
            }
        }

        private Dropped GetRandomDrop(int level)
        {
            Dropped drop;
            DropConfig config = SelectRandomItem(level);
            drop.item = config.item;
            drop.number = config.GetRandomNumber(level);
            return drop;
        }

        private int GetRandomNumberOfDrops(int level)
        {
            return Random.Range(GetByLevel(minDrops, level), GetByLevel(maxDrops, level));
        }

        private bool ShouldRandomDrop(int level)
        {
            return Random.Range(0, 100) < GetByLevel(dropChancePercentage, level);
        }

        DropConfig SelectRandomItem(int level)
        {
            float totalChance = GetTotalChance(level);
            float randomRoll = Random.Range(0, totalChance);
            float currTotal = 0;
            foreach(var drop in potentialDrops)
            {
                currTotal += GetByLevel(drop.relativeChance, level);
                if (randomRoll < currTotal)
                    return drop;
            }
            return null;
        }

        private float GetTotalChance(int level)
        {
            float currTotal = 0;
            foreach (DropConfig config in potentialDrops)
                currTotal += GetByLevel(config.relativeChance, level);

            return currTotal;
        }

        static T GetByLevel<T>(T[] values, int level)
        {
            if (values.Length == 0)
                return default;
            if (level > values.Length)
                return values[values.Length - 1];
            if (level <= 0)
                return default;

            return values[level - 1];
        }
    }
}