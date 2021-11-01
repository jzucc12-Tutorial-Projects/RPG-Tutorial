using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Control;
using UnityEngine;

namespace GameDevTV.Assets.Dialogues
{
    public class AIConversant : MonoBehaviour, IRaycastable
    {
        [SerializeField] Dialogue myDialogue = null;
        [SerializeField] string myName = "Guard";

        public CursorType GetCursorType()
        {
            return CursorType.Dialogue;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if(myDialogue == null) 
                return false;

            if(GetComponent<Health>() && !GetComponent<Health>().IsAlive())
                return false;
            
            if(Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<PlayerConversant>().StartDialogue(this, myDialogue);
            }
            return true;
        }

        public string GetName() { return myName; }
    }
}
