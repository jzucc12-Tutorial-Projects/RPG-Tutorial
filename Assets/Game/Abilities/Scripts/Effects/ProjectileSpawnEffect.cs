using System;
using System.Collections;
using RPG.Attributes;
using RPG.Combat;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Projectile Spawn Effect", menuName = "Abilities/Effects/Projectile Spawn Effect", order = 0)]
    public class ProjectileSpawnEffect : EffectStrategy
    {
        [SerializeField] Projectile projectileToSpawn = null;
        [SerializeField] float damage = 0;
        [SerializeField] bool isRightHand = true;
        [SerializeField] bool useTargetPoint = true;

        public override void StartEffect(AbilityData data, Action finished)
        {
            Fighter fighter = data.GetUser().GetComponent<Fighter>();
            Vector3 spawnPosition = fighter.GetHandTransform(isRightHand).position;
            if(useTargetPoint) SpawnProjectilesToPoint(data, spawnPosition);
            else SpawnProjectilesToTarget(data, spawnPosition);

            finished?.Invoke();
        }

        void SpawnProjectilesToTarget(AbilityData data, Vector3 spawnPosition)
        {
            Projectile projectile = Instantiate(projectileToSpawn);
            projectile.transform.position = spawnPosition;
            projectile.SetTarget(data.GetPoint(), data.GetUser(), damage);
        }

        void SpawnProjectilesToPoint(AbilityData data, Vector3 spawnPosition)
        {
            foreach(GameObject target in data.GetTargets())
            {
                Health health = target.GetComponent<Health>();
                if(health == null) continue;
                
                Projectile projectile = Instantiate(projectileToSpawn);
                projectile.transform.position = spawnPosition;
                projectile.SetTarget(health, data.GetUser(), damage);
            }
        }
    }
}
