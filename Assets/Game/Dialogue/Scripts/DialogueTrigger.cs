using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameDevTV.Assets.Dialogues
{
    public class DialogueTrigger : MonoBehaviour
    {
        [SerializeField] string action = null;
        [SerializeField] UnityEvent onTrigger = null;

        public void Trigger(string actionTrigger)
        {
            if(action != actionTrigger) return;

            onTrigger?.Invoke();
        }        
    }

}