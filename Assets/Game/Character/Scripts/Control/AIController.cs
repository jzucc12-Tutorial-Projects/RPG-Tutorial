using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Move;
using System;
using RPG.Attributes;
using GameDevTV.Utils;
using UnityEngine.AI;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicousTime = 2f;
        [SerializeField] float aggroCooldownTime = 3f;
        [SerializeField] ControlPath patrolPath = null;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float dwellTime = 1f;
        [Range(0,1)][SerializeField] float patrolSpeedFraction = 0.2f;
        [SerializeField] float shoutDistance = 5;
        GameObject player;
        Fighter myFighter;
        Health myHealth;
        Mover myMover;

        LazyValue<Vector3> guardLocation;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        int currentWaypoint = 0;
        float timeSinceWaypointArrival = Mathf.Infinity;
        float timeSinceAggrevated = Mathf.Infinity;

        private void Awake()
        {
            player = GameObject.FindWithTag("Player");
            myFighter = GetComponent<Fighter>();
            myHealth = GetComponent<Health>();
            myMover = GetComponent<Mover>();
            guardLocation = new LazyValue<Vector3>(GetGuardPosition);
            guardLocation.ForceInit();
        }

        private void Update()
        {
            if (!myHealth.IsAlive()) return;
            if (IsAggrevataed() && myFighter.CanAttack(player))
            {
                Aggrevate();
                AttackBehavior();
            }
            else if (timeSinceLastSawPlayer <= suspicousTime)
            {
                SuspiciousBehavior();
            }
            else
            {
                GuardBehavior();
            }

            UpdateTimers();
        }

        Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        public void ResetEnemy()
        {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            agent.Warp(guardLocation.value);
            timeSinceWaypointArrival = Mathf.Infinity;
            timeSinceAggrevated = Mathf.Infinity;
            timeSinceLastSawPlayer = Mathf.Infinity;
            currentWaypoint = 0;
        }

        public void Aggrevate()
        {
            timeSinceAggrevated = 0;
            AggrevateNearbyEnemies();
        }

        void AggrevateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
            foreach(RaycastHit hit in hits)
            {
                AIController ai = hit.collider.GetComponent<AIController>();
                if (!ai) continue;
                if (ai.IsAggrevataed()) continue;
                ai.Aggrevate();
            }
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceWaypointArrival += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;
        }

        private void AttackBehavior()
        {
            timeSinceLastSawPlayer = 0f;
            myFighter.Attack(player);
        }

        private void SuspiciousBehavior()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void GuardBehavior()
        {
            Vector3 nextPosition = guardLocation.value;
            if(patrolPath != null)
            {
                if(AtWaypoint())
                {
                    timeSinceWaypointArrival = 0;
                   CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }

            if (timeSinceWaypointArrival > dwellTime)
            {
                myMover.StartMoveAction(nextPosition,patrolSpeedFraction);
            }
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypoint);
        }

        private void CycleWaypoint()
        {
            currentWaypoint = patrolPath.GetNextJJ(currentWaypoint);
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private bool IsAggrevataed()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            return distanceToPlayer <= chaseDistance || timeSinceAggrevated < aggroCooldownTime;
        }


        //Called by Unity
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}

