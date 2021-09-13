using RPG.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] RectTransform greenBar = null;
        [SerializeField] Health health = null;

        void Update()
        {
            float percent = health.GetPercentage() / 100f;
            greenBar.localScale = new Vector3(percent, 1, 1);
            if (percent <= 0)
                gameObject.SetActive(false);
        }
    }
}
