using System.Collections.Generic;
using RPG.Characters;
using RPG.Combat;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.CharacterControl
{
    [RequireComponent(typeof(Fighter)), RequireComponent(typeof(Mover))]
    public class AIController : MonoBehaviour, ISaveable
    {
        [SerializeField]
        float chaseDistance = 5f;

        [SerializeField]
        float suspicionTime = 5f;

        [SerializeField]
        float waypointPrecision = 0.5f;

        [SerializeField]
        float waypointDwellDuration = 3f;

        [SerializeField]
        float patrolSpeed = 2.5f;

        [SerializeField]
        float chaseSpeed = 3.5f;

        [SerializeField]
        PatrolPath patrolPath;

        Fighter fighter;
        Health health;
        GameObject player;
        ActionScheduler scheduler;
        Mover mover;

        Vector3 guardPosition;
        int currentWaypointIndex = 0;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceLastWaypoint = 0;

        private void Start()
        {
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            scheduler = GetComponent<ActionScheduler>();
            player = GameObject.FindWithTag("Player");
            guardPosition = transform.position;
        }

        private void Update()
        {
            if (health.IsDead)
                return;

            timeSinceLastSawPlayer += Time.deltaTime;

            if (Vector3.Distance(transform.position, player.transform.position) <= chaseDistance)
            {
                fighter.Attack(player.GetComponent<Health>());
                timeSinceLastSawPlayer = 0;
                mover.SetSpeed(chaseSpeed);
            }
            else if (timeSinceLastSawPlayer <= suspicionTime)
            {
                scheduler.CancelCurrentAction();
            }
            else
            {
                Vector3 moveTo = guardPosition;
                mover.SetSpeed(patrolSpeed);
                if (patrolPath is not null && patrolPath.transform.childCount > 0)
                {
                    Transform currentWaypoint = patrolPath.GetWaypoint(currentWaypointIndex);
                    moveTo = currentWaypoint.position;
                    if (
                        Vector3.Distance(transform.position, currentWaypoint.position)
                        < waypointPrecision
                    )
                    {
                        if (timeSinceLastWaypoint > waypointDwellDuration)
                        {
                            currentWaypointIndex++;
                            timeSinceLastWaypoint = 0;
                        }
                        else
                        {
                            timeSinceLastWaypoint += Time.deltaTime;
                        }
                    }
                }
                mover.StartMoveAction(moveTo);
            }
        }

        // Called by Unity
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }

        public object CaptureState()
        {
            Dictionary<string, float> varsDict = new Dictionary<string, float>();

            varsDict["currentWaypointIndex"] = currentWaypointIndex;
            varsDict["timeSinceLastSawPlayer"] = timeSinceLastSawPlayer;
            varsDict["timeSinceLastWaypoint"] = timeSinceLastWaypoint;

            return varsDict;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, float> varsDict = (Dictionary<string, float>)state;

            currentWaypointIndex = (int)varsDict["currentWaypointIndex"];
            timeSinceLastSawPlayer = varsDict["timeSinceLastSawPlayer"];
            timeSinceLastWaypoint = varsDict["timeSinceLastWaypoint"];
        }
    }
}
