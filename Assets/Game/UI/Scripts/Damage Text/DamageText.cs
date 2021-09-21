using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.DamageText
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] Text myText = null;

        public void SetDmg(int dmg)
        {
            myText.text = dmg.ToString();
        }
    }
}