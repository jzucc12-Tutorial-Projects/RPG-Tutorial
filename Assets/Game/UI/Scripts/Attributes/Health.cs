using GameDevTV.Saving;
using RPG.Stats;
using UnityEngine;
using RPG.Core;
using GameDevTV.Utils;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] LazyValue<int> health;
        [SerializeField] float regenPercent = 70;
        [SerializeField] UnityEvent<int> DamageTaken = null;
        [SerializeField] float respawnPercentage = 0.5f;
        public UnityEvent OnDie = null;

        bool isAlive = true;

        private void Awake()
        {
            health = new LazyValue<int>(GetInitialHealth);
        }

        int GetInitialHealth()
        {
            return GetMaxHealth();
        }

        private void Start()
        {
            health.ForceInit();
        }

        private void OnEnable()
        {
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>().onLevelUp -= RegenerateHealth;
        }

        public object CaptureState()
        {
            return health.value;
        }

        public bool IsAlive()
        {
            return health.value > 0;
        }

        public void Respawn()
        {
            Heal((int)(GetMaxHealth() * respawnPercentage));
            Revive();
        }

        public void RestoreState(object state)
        {
            health.value = (int)state;
            if (!IsAlive())
            {
                Die();
            }
        }

        internal void Heal(int hpRestore)
        {
            health.value = Mathf.Min(health.value + hpRestore, GetMaxHealth());
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            int roundDamage = Mathf.FloorToInt(damage);
            health.value = Mathf.Max(0, health.value - roundDamage);
            if (!IsAlive())
            {
                OnDie?.Invoke();
                Die();
                AwardExperience(instigator);
            }
            else
                DamageTaken?.Invoke(roundDamage);
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) return;
            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        private void RegenerateHealth()
        {
            int regenHP = Mathf.RoundToInt(GetComponent<BaseStats>().GetStat(Stat.Health) * regenPercent/100);
            health.value = Mathf.Max(health.value, regenHP);
        }

        public int GetMaxHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GetPercentage()
        {
            float currHealth = health.value;
            return currHealth / GetMaxHealth()  * 100;
        }

        public string GetPercentageAsFraction()
        {
            float currHealth = health.value;
            return currHealth.ToString() + " / " + GetMaxHealth().ToString();
        }

        private void Die()
        {
            if (!IsAlive()) return;
            GetComponent<Animator>().SetTrigger("Die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        void Revive()
        {
            if(IsAlive()) return;
            GetComponent<Animator>().Rebind();
            isAlive = true;
        }
    }
}
