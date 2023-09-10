using System.Collections.Generic;
using RPG.Characters;
using RPG.Combat;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;
using UnityEngine.Events;

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
        float aggroTime = 5f;

        [SerializeField]
        float waypointPrecision = 0.5f;

        [SerializeField]
        float waypointDwellDuration = 3f;

        [SerializeField]
        float patrolSpeed = 2.5f;

        [SerializeField]
        float chaseSpeed = 3.5f;

        [SerializeField]
        float shoutDistance = 5f;

        [SerializeField]
        PatrolPath patrolPath;

        Fighter fighter;
        Health health;
        GameObject player;
        ActionScheduler scheduler;
        Mover mover;

        Vector3 guardPosition;
        int currentWaypointIndex = 0;
        float timeSinceAggro = Mathf.Infinity;
        float timeSinceLastSeenPlayer = Mathf.Infinity;
        float timeSinceLastWaypoint = 0;
        bool aggro = false;

        public bool Aggro
        {
            get { return aggro; }
            set
            {
                if (!aggro && value)
                {
                    TriggerAggro?.Invoke();
                    AggroNearbyEnemies();
                }
                if (value)
                {
                    timeSinceAggro = 0;
                }
                if (value != aggro)
                {
                    timeSinceLastSeenPlayer = 0; // reset timeSinceLastSeenPlayer in order to stay suspicious
                }
                aggro = value;
            }
        }

        [SerializeField]
        UnityEvent TriggerAggro;

        private void Awake()
        {
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            scheduler = GetComponent<ActionScheduler>();
        }

        private void OnEnable()
        {
            health.takeDamage.AddListener(
                (float value) =>
                {
                    Aggro = true;
                }
            );
        }

        private void Start()
        {
            player = GameObject.FindWithTag("Player");
            guardPosition = transform.position;
        }

        private void Update()
        {
            if (health.IsDead)
                return;

            timeSinceAggro += Time.deltaTime;
            timeSinceLastSeenPlayer += Time.deltaTime;

            if (Vector3.Distance(transform.position, player.transform.position) <= chaseDistance)
            {
                Aggro = true;
            }

            if (Aggro)
            {
                fighter.Attack(player.GetComponent<Health>());
                mover.SetSpeed(chaseSpeed);
            }
            else if (timeSinceLastSeenPlayer <= suspicionTime)
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

            if (timeSinceAggro > aggroTime && Aggro)
            {
                Aggro = false;
            }
        }

        // Called by Unity
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }

        private void AggroNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(
                transform.position,
                shoutDistance,
                Vector3.up,
                0f
            );
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.TryGetComponent(out AIController enemyController))
                {
                    print($"enemy found by {hit.collider.gameObject}");
                    enemyController.Aggro = true;
                }
            }
        }

        public object CaptureState()
        {
            Dictionary<string, float> varsDict = new Dictionary<string, float>();

            varsDict["currentWaypointIndex"] = currentWaypointIndex;
            varsDict["timeSinceLastSawPlayer"] = timeSinceAggro;
            varsDict["timeSinceLastWaypoint"] = timeSinceLastWaypoint;

            return varsDict;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, float> varsDict = (Dictionary<string, float>)state;

            currentWaypointIndex = (int)varsDict["currentWaypointIndex"];
            timeSinceAggro = varsDict["timeSinceLastSawPlayer"];
            timeSinceLastWaypoint = varsDict["timeSinceLastWaypoint"];
        }
    }
}
