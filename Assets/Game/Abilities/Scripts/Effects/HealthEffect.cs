using System;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Health Effect", menuName = "Abilities/Effects/Health Effect", order = 0)]
    public class HealthEffect : EffectStrategy
    {
        [SerializeField] float damage = 0;

        public override void StartEffect(AbilityData data, Action finished)
        {
            foreach(var target in data.GetTargets())
            {
                Health health = target.GetComponent<Health>();
                if(health == null) continue;

                if(damage > 0)
                    health.TakeDamage(data.GetUser(), damage);
                else
                    health.Heal((int)Mathf.Abs(damage));
            }
        }
    }
}
