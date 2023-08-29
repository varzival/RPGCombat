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
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        NavMeshAgent agent;
        Health health;

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }

        public void StartMoveAction(Vector3 to)
        {
            MoveTo(to);
            GetComponent<ActionScheduler>().StartAction(this);
        }

        public void MoveTo(Vector3 to)
        {
            agent.isStopped = false;
            agent.destination = to;
        }

        public void SetSpeed(float speed)
        {
            agent.speed = speed;
        }

        public void Cancel()
        {
            agent.isStopped = true;
        }

        // Update is called once per frame
        void Update()
        {
            agent.enabled = !health.IsDead;

            UpdateAnimator();
        }

        private void UpdateAnimator()
        {
            Vector3 localVelocity = transform.worldToLocalMatrix.MultiplyVector(agent.velocity);
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
            GetComponent<NavMeshAgent>().Warp(vector3.toVector3());
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}
