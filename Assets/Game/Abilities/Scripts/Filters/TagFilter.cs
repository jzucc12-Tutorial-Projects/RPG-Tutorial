using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Filter
{
    [CreateAssetMenu(fileName = "Tag Filter", menuName = "Abilities/Filters/Tag Filter", order = 0)]
    public class TagFilter : FilterStrategy
    {
        [SerializeField] string tagToFilter = "";

        public override IEnumerable<GameObject> Filter(IEnumerable<GameObject> toFilter)
        {
            foreach(var target in toFilter)
            {
                if(!target.CompareTag(tagToFilter)) continue;
                yield return target;
            }
        }
    }
}