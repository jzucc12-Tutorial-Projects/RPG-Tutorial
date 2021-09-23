using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class AggroGroup : MonoBehaviour
    {
        [SerializeField] Fighter[] fighters = null;
        [SerializeField] bool activateOnStart = false;

        private void Start() 
        {
            Activate(activateOnStart);    
        }
        
        public void Activate(bool shouldActivate)
        {
            foreach(Fighter fighter in fighters)
            {
                var combatTarget = fighter.GetComponent<CombatTarget>();
                if(combatTarget)
                    combatTarget.enabled = shouldActivate;

                fighter.enabled = shouldActivate;
            }
        }
    }
}
