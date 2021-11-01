using System;
using System.Collections;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Trigger Animation Effect", menuName = "Abilities/Effects/Trigger Animation Effect", order = 0)]
    public class TriggerAnimationEffect : EffectStrategy
    {
        [SerializeField] string triggerName = "";

        public override void StartEffect(AbilityData data, Action finished)
        {
            data.GetUser().GetComponent<Animator>().SetTrigger(triggerName);
        }
    }
}
