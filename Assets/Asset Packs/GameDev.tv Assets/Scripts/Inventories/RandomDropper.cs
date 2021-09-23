using GameDevTV.Inventories;
using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Inventories
{
    public class RandomDropper : ItemDropper
    {
        [Tooltip("How far can the pickups be scattered from the dropper.")]
        [SerializeField] float scatterDistance = 1;
        [SerializeField] DropLibrary dropLibrary = null;

        const int ATTEMPTS = 30;

        public void RandomDrop()
        {
            var baseStats = GetComponent<BaseStats>();
            var drops = dropLibrary.GetRandomDrops(baseStats.GetLevel());
            
            foreach(var drop in drops)
                DropItem(drop.item, drop.number);
        }

        protected override Vector3 GetDropLocation()
        {
            for(int ii = 0; ii < ATTEMPTS; ii++)
            {
                Vector3 randomPoint = transform.position + Random.insideUnitSphere * scatterDistance;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 0.1f, NavMesh.AllAreas))
                    return hit.position;
            }

            Debug.Log("nope");
            return transform.position;
        }
    }
}
