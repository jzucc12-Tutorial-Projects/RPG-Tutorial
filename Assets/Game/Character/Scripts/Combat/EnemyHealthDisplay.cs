using UnityEngine;
using UnityEngine.UI;
using System;
using RPG.Attributes;

namespace RPG.Combat
{ 
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter fighter;

        private void Awake()
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        private void Update()
        {
            Health health = fighter.GetTarget();
            GetComponent<Text>().text = (health ? String.Format("{0:0}%", health.GetPercentage()) : "N/A");

        }
    }
}
