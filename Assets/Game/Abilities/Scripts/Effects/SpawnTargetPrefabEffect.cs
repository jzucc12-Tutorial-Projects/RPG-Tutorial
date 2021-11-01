using System;
using System.Collections;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Spawn Target Prefab Effect", menuName = "Abilities/Effects/Spawn Target Prefab Effect", order = 0)]
    public class SpawnTargetPrefabEffect : EffectStrategy
    {
        [SerializeField] Transform prefabToSpawn = null;
        [SerializeField] float destroyDelay = -1;

        public override void StartEffect(AbilityData data, Action finished)
        {
            Transform spawned = Instantiate(prefabToSpawn, data.GetPoint(), Quaternion.identity);
            if(destroyDelay < 0)
            {
                finished?.Invoke();
                return;
            }
            data.StartCoroutine(DestroyDelay(spawned, finished));
        }

        IEnumerator DestroyDelay(Transform toDestroy, Action finished)
        {
            yield return new WaitForSeconds(destroyDelay);
            Destroy(toDestroy.gameObject);
            finished?.Invoke();
        }
    }
}
