using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Abilities
{
    public class AbilityData : IAction
    {
        GameObject user;
        Vector3 point;
        IEnumerable<GameObject> targets;
        bool cancelled = false;

        public AbilityData(GameObject user)
        {
            this.user = user;
        }

        public GameObject GetUser() { return user; }
        public Vector3 GetPoint() { return point; }
        public IEnumerable<GameObject> GetTargets() { return targets; }
        public void SetTargets(IEnumerable<GameObject> targets) { this.targets = targets; }
        public void SetPoint(Vector3 point) { this.point = point; }

        public void StartCoroutine(IEnumerator coroutine)
        {
            user.GetComponent<MonoBehaviour>().StartCoroutine(coroutine);
        }

        public void Cancel()
        {
            cancelled = true;
        }

        public bool IsCancelled() { return cancelled; }
    }
}