using GameDevTV.Saving;
using System;
using UnityEngine;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencePoints = 0;
        //public delegate void ExperienceGainedDelegate();
        public event Action onExperienceGained;

        public float GetPoints() { return experiencePoints; }

        public object CaptureState()
        {
            return experiencePoints;
        }

        public void GainExperience(float experience)
        {
            experiencePoints += experience;
            onExperienceGained();
        }

        public void RestoreState(object state)
        {
            experiencePoints = (float)state;
        }

        private void Update() 
        {
            if(Input.GetKey(KeyCode.E))
                GainExperience(Time.deltaTime * 1000);
        }
    }
}
