using UnityEngine;
using UnityEngine.UI;
using System;
using RPG.Stats;

namespace RPG.Attributes
{
    public class EXPDisplay : MonoBehaviour
    {
        Experience exp;

        private void Awake()
        {
            exp = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        private void Update()
        {
            GetComponent<Text>().text = exp.GetPoints().ToString();

        }
    }
}
