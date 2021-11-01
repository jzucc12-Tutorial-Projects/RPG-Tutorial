using UnityEngine;
using UnityEngine.UI;
using System;

namespace RPG.Attributes
{
    public class ManaDisplay : MonoBehaviour
    {
        Mana mana;

        private void Awake()
        {
            mana = GameObject.FindWithTag("Player").GetComponent<Mana>();
        }

        private void Update()
        {
            GetComponent<Text>().text = $"{mana.GetMana():N0} / {mana.GetMaxMana()}";
        }
    }
}
