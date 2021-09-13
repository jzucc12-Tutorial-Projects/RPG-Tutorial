using UnityEngine;
using UnityEngine.UI;
using System;
using RPG.Stats;

namespace RPG.Attributes
{
    public class LevelDisplay : MonoBehaviour
    {
        BaseStats stats;

        private void Awake()
        {
            stats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }

        private void Update()
        {
            GetComponent<Text>().text = stats.GetLevel().ToString();
        }
    }
}
