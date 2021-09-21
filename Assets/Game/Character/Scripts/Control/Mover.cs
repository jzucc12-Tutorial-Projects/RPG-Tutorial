using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using GameDevTV.Saving;
using RPG.Attributes;

namespace RPG.Move
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] float maxSpeed = 6f;
        [SerializeField] float maxPathLength = 40;
        NavMeshAgent myNavMeshAgent;
        Health myHealth;

        private void Start()
        {
            myNavMeshAgent = GetComponent<NavMeshAgent>();
            myHealth = GetComponent<Health>();
        }

        void Update()
        {
            myNavMeshAgent.enabled = myHealth.IsAlive();
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLength(path) > maxPathLength) return false;

            return true;
        }

        private float GetPathLength(NavMeshPath path)
        {
            float distance = 0;
            if (path.corners.Length < 2) return distance;

            for (int ii = 0; ii < path.corners.Length - 1; ii++)
            {
                distance += Vector3.Distance(path.corners[ii], path.corners[ii + 1]);
            }

            return distance;
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            myNavMeshAgent.destination = destination;
            myNavMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            myNavMeshAgent.isStopped = false;
        }

        public void Cancel()
        {
            myNavMeshAgent.isStopped = true;
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = myNavMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("Forward Speed", speed);
        }

        [System.Serializable]
        struct MoverSaveData
        {
            public SerializableVector3 position;
            public SerializableVector3 rotation;
        }

        public object CaptureState()
        {
            MoverSaveData data = new MoverSaveData();
            data.position = new SerializableVector3(transform.position);
            data.rotation = new SerializableVector3(transform.eulerAngles);
            return data;
        }

        public void RestoreState(object state)
        {
            MoverSaveData data = (MoverSaveData)state;
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = data.position.ToVector();
            transform.eulerAngles = data.rotation.ToVector();
            GetComponent<NavMeshAgent>().enabled = true;
        }
    }
}

