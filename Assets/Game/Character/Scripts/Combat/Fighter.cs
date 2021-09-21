using System.Collections.Generic;
using UnityEngine;
using RPG.Move;
using RPG.Core;
using GameDevTV.Saving;
using RPG.Attributes;
using RPG.Stats;
using GameDevTV.Utils;
using GameDevTV.Inventories;
using System;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeaponConfig = null;

        Health target;
        Equipment equipment;
        float timeSinceLastAttack = Mathf.Infinity;
        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;

        private void Awake()
        {
            currentWeaponConfig = defaultWeaponConfig;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
            equipment = GetComponent<Equipment>();
            if (equipment)
                equipment.equipmentUpdated += UpdateWeapon;
        }

        private void Start()
        {
            currentWeapon.ForceInit();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (!target) return;
            if (!target.IsAlive()) return;

            if (!GetIsInRange(target.transform))
            {
                GetComponent<Mover>().MoveTo(target.transform.position,1f);
            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }

        Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeaponConfig);
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            if (!GetComponent<Mover>().CanMoveTo(combatTarget.transform.position) &&
                !GetIsInRange(combatTarget.transform)) return false;
            Health test = combatTarget.GetComponent<Health>();
            return (test && test.IsAlive());
        }

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack >= currentWeaponConfig.GetSpeed())
            {
                GetComponent<Animator>().ResetTrigger("Stop Attack");
                GetComponent<Animator>().SetTrigger("Attack");
                timeSinceLastAttack = 0f;
            }
        }

        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) <= currentWeaponConfig.GetRange();
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            GetComponent<Animator>().ResetTrigger("Attack");
            GetComponent<Animator>().SetTrigger("Stop Attack");
            GetComponent<Mover>().Cancel();
            target = null;
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        private void UpdateWeapon()
        {
            WeaponConfig weapon = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            if (!weapon) weapon = defaultWeaponConfig;
            EquipWeapon(weapon);

        }


        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            return weapon.Spawn(rightHandTransform, leftHandTransform, GetComponent<Animator>());
        }

        public Health GetTarget()
        {
            return target;
        }

        #region //Animation Event
        void Hit()
        {
            if (!target) return;
            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

            if(currentWeapon.value)
            {
                currentWeapon.value.OnHit();
            }

            if (currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            }
            else
                target.TakeDamage(gameObject,damage);

        }

        void Shoot()
        {
            Hit();
        }
        #endregion

        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            WeaponConfig weapon = UnityEngine.Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }
    }
}
