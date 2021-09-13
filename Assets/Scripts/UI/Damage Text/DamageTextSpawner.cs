using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] DamageText dmgTextPrefab = null;

        public void Spawn(int damageAmount)
        {
            var dmgText = Instantiate<DamageText>(dmgTextPrefab, transform);
            dmgText.SetDmg(damageAmount);
        }
    }
}
