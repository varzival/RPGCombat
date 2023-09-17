using System.Linq;
using Newtonsoft.Json.Linq;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Characters
{
    [
        RequireComponent(typeof(ActionScheduler)),
        RequireComponent(typeof(Health)),
        RequireComponent(typeof(NavMeshAgent))
    ]
    public class Mover : MonoBehaviour, IAction, ISaveable, IJSONSaveable
    {
        NavMeshAgent navMeshAgent;
        Health health;
        ActionScheduler actionScheduler;

        [SerializeField]
        float maxNavMeshDistance = 10f;

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
            actionScheduler = GetComponent<ActionScheduler>();
        }

        private float GetPathLength(NavMeshPath path)
        {
            if (path.corners?.Length < 2)
                return 0;
            Vector3 lastCorner = Vector3.zero;
            return path.corners.Aggregate(
                0f,
                (current, next) =>
                {
                    if (lastCorner == Vector3.zero)
                    {
                        lastCorner = next;
                        return current;
                    }
                    else
                    {
                        current += Vector3.Distance(lastCorner, next);
                        lastCorner = next;
                        return current;
                    }
                }
            );
        }

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new();
            if (NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path))
            {
                return path.status == NavMeshPathStatus.PathComplete
                    && GetPathLength(path) <= maxNavMeshDistance;
            }
            return false;
        }

        public void StartMoveAction(Vector3 to)
        {
            MoveTo(to);
            actionScheduler.StartAction(this);
        }

        public void MoveTo(Vector3 to)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.destination = to;
        }

        public void SetSpeed(float speed)
        {
            navMeshAgent.speed = speed;
        }

        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }

        // Update is called once per frame
        void Update()
        {
            navMeshAgent.enabled = !health.IsDead;

            UpdateAnimator();
        }

        private void UpdateAnimator()
        {
            Vector3 localVelocity = transform.worldToLocalMatrix.MultiplyVector(
                navMeshAgent.velocity
            );
            //Vector3 localVelocity = transform.InverseTransformDirection(GetComponent<NavMeshAgent>().velocity);
            GetComponent<Animator>()
                .SetFloat("ForwardSpeed", localVelocity.z);
        }

        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            SerializableVector3 vector3 = (SerializableVector3)state;
            navMeshAgent.Warp(vector3.toVector3());
            actionScheduler.CancelCurrentAction();
        }

        public JToken CaptureStateAsJToken()
        {
            return transform.position.ToToken();
        }

        public void RestoreStateFromJToken(JToken state)
        {
            Vector3 vector3 = state.ToVector3();
            navMeshAgent.Warp(vector3);
            actionScheduler.CancelCurrentAction();
        }
    }
}
