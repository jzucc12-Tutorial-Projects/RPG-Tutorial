using System;
using GameDevTV.Saving;
using UnityEngine;

namespace GameDevTV.Inventories
{
    public class Purse : MonoBehaviour, ISaveable
    {
        [SerializeField] float startingBalance = 400f;
        public event Action OnChange;
        float balance = 0;

        private void Awake() 
        {
            balance = startingBalance;
        }

        public float GetBalance()
        {
            return balance;
        }

        public void UpdateBalance(float amount)
        {
            balance += amount;
            OnChange?.Invoke();
        }

        public object CaptureState()
        {
            return balance;
        }

        public void RestoreState(object state)
        {
            balance = (float)state;
        }
    }
}