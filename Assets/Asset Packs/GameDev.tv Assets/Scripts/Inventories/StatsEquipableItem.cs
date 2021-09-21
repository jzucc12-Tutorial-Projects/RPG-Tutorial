using GameDevTV.Inventories;
using RPG.Stats;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = ("RPG/Inventory/Equipable Item"))]
    public class StatsEquipableItem : EquipableItem, IModifierProvider
    {
        [SerializeField] Modifer[] additiveModifier = null;
        [SerializeField] Modifer[] percentageModifier = null;

        [System.Serializable]
        struct Modifer
        {
            public Stat stat;
            public float value;
        }

        public IEnumerable<float> GetAdditiveModifier(Stat stat)
        {
            foreach(Modifer mod in additiveModifier)
            {
                if (mod.stat != stat) continue;
                yield return mod.value;
            }
        }

        public IEnumerable<float> GetPercentageModifier(Stat stat)
        {
            foreach (Modifer mod in percentageModifier)
            {
                if (mod.stat != stat) continue;
                yield return mod.value;
            }
        }

    }
}
