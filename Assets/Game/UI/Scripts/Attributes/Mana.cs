using System.Collections;
using System.Collections.Generic;
using GameDevTV.Saving;
using GameDevTV.Utils;
using RPG.Stats;
using UnityEngine;

namespace RPG.Attributes
{
    public class Mana : MonoBehaviour, ISaveable
    {
        LazyValue<float> mana;


        private void Awake() 
        {
            mana = new LazyValue<float>(GetMaxMana); 
        }

        private void Update() 
        {
            if(mana.value < GetMaxMana())
            {
                RegenMana(GetRegenRate() * Time.deltaTime);
            }
        }

        public float GetMana()
        {
            return mana.value;
        }

        public float GetMaxMana()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Mana);
        }

        float GetRegenRate()
        {
            return GetComponent<BaseStats>().GetStat(Stat.ManaRegenRate); 
        }

        public bool UseMana(float manaToUse)
        {
            if(manaToUse > mana.value)
                return false;

            mana.value -= manaToUse;
            return true;
        }

        public void RegenMana(float amount)
        {
            mana.value = Mathf.Min(GetMaxMana(), mana.value + amount);
        }

        public object CaptureState()
        {
            return mana.value;
        }

        public void RestoreState(object state)
        {
            mana.value = (float)state;
        }
    }
}