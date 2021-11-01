using System;
using System.Collections;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Delay Composite Effect", menuName = "Abilities/Effects/Delay Composite Effect", order = 0)]
    public class DelayCompositeEffect : EffectStrategy
    {
        [SerializeField] float delay = 0;
        [SerializeField] EffectStrategy[] delayedEffects;
        [SerializeField] bool abortIfCancelled = false;

        public override void StartEffect(AbilityData data, Action finished)
        {
            data.StartCoroutine(Delay(data, finished));
        }

        IEnumerator Delay(AbilityData data, Action finished)
        {
            yield return new WaitForSeconds(delay);
            if(abortIfCancelled & data.IsCancelled()) yield break;

            foreach(var strategy in delayedEffects)
                strategy.StartEffect(data, finished);
        }
    }
}
