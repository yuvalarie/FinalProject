using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Npc
{
    [RequireComponent(typeof(Rigidbody2D))] // Forces Unity to add a Rigidbody2D
    public class RoamingNpcController : MonoBehaviour
    {
        [Header("Movement Settings")] 
        [Tooltip("Drag empty GameObjects here to define the path.")] 
        [SerializeField] protected Transform[] waypoints;

        [SerializeField] protected float moveSpeed = 2.0f;
        [SerializeField] protected float waitTimeAtPoint = 1.0f;
        [SerializeField] protected bool randomPatrol;
        
        [Tooltip("How far the NPC wanders from its current spot if no waypoints are set.")]
        [SerializeField] protected float wanderRadius = 3f;

        private int _currentWaypointIndex = 0;
        private float _waitTimer;
        private bool _isWaiting;
        private Vector2 _currentTargetPosition;
        private Rigidbody2D _rb;
        
        public bool Roaming { get; set; } = true;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            if (waypoints != null && waypoints.Length > 0)
            {
                _currentTargetPosition = waypoints[_currentWaypointIndex].position;
            }
            else
            {
                GenerateRandomWanderPoint();
            }
        }

        // Physics movement MUST happen in FixedUpdate
        private void FixedUpdate()
        {
            HandlePatrolPhysics();
        }

        private void HandlePatrolPhysics()
        {
            if (!Roaming) 
            {
                // Instantly stop horizontal movement if grabbed, but keep gravity
                _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
                return;
            }

            if (_isWaiting)
            {
                _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
                _waitTimer += Time.fixedDeltaTime;
                if (_waitTimer >= waitTimeAtPoint)
                {
                    PickNextWaypoint();
                }
                return;
            }

            // Calculate which way to walk on the X axis
            float directionX = _currentTargetPosition.x - transform.position.x;
            
            // If we are far enough away, walk towards it using velocity
            if (Mathf.Abs(directionX) > 0.1f)
            {
                float moveDirection = Mathf.Sign(directionX); // Returns 1 for right, -1 for left
                _rb.linearVelocity = new Vector2(moveDirection * moveSpeed, _rb.linearVelocity.y);
            }
            else
            {
                _isWaiting = true;
            }
        }

        // If the NPC walks into a wall before reaching its destination, give up and wait
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (Roaming && !_isWaiting)
            {
                // You can check tags here if you only want it to bounce off specific walls
                _isWaiting = true; 
            }
        }

        private void PickNextWaypoint()
        {
            _isWaiting = false;
            _waitTimer = 0f;

            if (waypoints != null && waypoints.Length > 0)
            {
                if (randomPatrol)
                {
                    int newIndex;
                    do
                    {
                        newIndex = Random.Range(0, waypoints.Length);
                    } while (waypoints.Length > 1 && newIndex == _currentWaypointIndex);

                    _currentWaypointIndex = newIndex;
                }
                else
                {
                    _currentWaypointIndex = (_currentWaypointIndex + 1) % waypoints.Length;
                }
                _currentTargetPosition = waypoints[_currentWaypointIndex].position;
            }
            else
            {
                GenerateRandomWanderPoint();
            }
        }
        
        private void GenerateRandomWanderPoint()
        {
            Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
            randomOffset.y = 0; // Keep it on the same vertical level

            _currentTargetPosition = (Vector2)transform.position + randomOffset;
        }
        
        public void ClearWaypoints()
        {
            waypoints = Array.Empty<Transform>();
        }
    }
}