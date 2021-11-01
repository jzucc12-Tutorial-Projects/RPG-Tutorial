using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Attributes;
using RPG.Core;
using UnityEngine;

namespace  RPG.Abilities
{
    [CreateAssetMenu(fileName = "My Ability", menuName = "Abilities/Ability", order = 0)]
    public class Ability : ActionItem
    {
        [SerializeField] TargetingStrategy targetingStrategy = null;
        [SerializeField] FilterStrategy[] filterStrategies = null;
        [SerializeField] EffectStrategy[] effectStrategies = null;
        [SerializeField] float cooldownTimer = 1;
        [SerializeField] float manaCost = 100;

        public override void Use(GameObject user)
        {
            CooldownStore cooldownStore = user.GetComponent<CooldownStore>();
            if(cooldownStore.IsCoolingdown(this)) return;
            
            Mana mana = user.GetComponent<Mana>();
            if(mana.GetMana() < manaCost) return;

            var scheduler = user.GetComponent<ActionScheduler>();

            AbilityData data = new AbilityData(user);
            scheduler.StartAction(data);
            targetingStrategy.StartTargeting(data, () => 
            {
                TargetAcquired(data);
            });
        }

        private void TargetAcquired(AbilityData data)
        {
            if(data.IsCancelled()) return;
            
            Mana mana = data.GetUser().GetComponent<Mana>();
            if(!mana.UseMana(manaCost)) return;

            foreach(var strategy in filterStrategies)
                data.SetTargets(strategy.Filter(data.GetTargets()));

            foreach(var effect in effectStrategies)
                effect.StartEffect(data, EffectFinished);
            
            CooldownStore cooldownStore = data.GetUser().GetComponent<CooldownStore>();
            cooldownStore.StartCooldown(this, cooldownTimer);
        }

        void EffectFinished()
        {

        }
    }
}
