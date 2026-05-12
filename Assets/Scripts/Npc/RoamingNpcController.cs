using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Npc
{
    [RequireComponent(typeof(Rigidbody2D))] // Forces Unity to add a Rigidbody2D
    public class RoamingNpcController : MonoBehaviour
    {
        public static readonly List<RoamingNpcController> AllNpcs = new List<RoamingNpcController>();
        
        [Header("Movement Settings")] 
        [Tooltip("Drag empty GameObjects here to define the path.")] 
        [SerializeField] protected Transform[] waypoints;
        [SerializeField] protected float moveSpeed = 2.0f;
        [SerializeField] protected float waitTimeAtPoint = 1.0f;
        [SerializeField] protected bool randomPatrol;
        [Tooltip("How far the NPC wanders from its current spot if no waypoints are set.")]
        [SerializeField] protected float wanderRadius = 3f;

        [Header("Vision Settings")]
        [SerializeField, Tooltip("How far the NPC can see.")] 
        private float viewDistance = 4f;
        [SerializeField, Tooltip("The angle of the vision cone.")] 
        private float viewAngle = 45f;
        
        private int _currentWaypointIndex = 0;
        private float _waitTimer;
        private bool _isWaiting;
        private Vector2 _currentTargetPosition;
        private Rigidbody2D _rb;
        private Vector2 _currentFacingDirection = Vector2.right;
        
        private Transform _playerTarget;
        
        public bool Roaming { get; set; } = true;
        
        private void OnEnable()
        {
            AllNpcs.Add(this);
        }
        
        private void OnDisable()
        {
            AllNpcs.Remove(this);
        }
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }
        
        private void Start()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Angel");
            if (playerObj != null)
            {
                _playerTarget = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("NPC cannot find the Player! Did you forget to tag your Player as 'Player'?");
            }
            if (waypoints != null && waypoints.Length > 0)
            {
                _currentTargetPosition = waypoints[_currentWaypointIndex].position;
            }
            else
            {
                GenerateRandomWanderPoint();
            }
        }

        private void FixedUpdate()
        {
            HandlePatrolPhysics();
        }

        private void HandlePatrolPhysics()
        {
            if (!Roaming) 
            {
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

            float directionX = _currentTargetPosition.x - transform.position.x;
            
            if (Mathf.Abs(directionX) > 0.1f)
            {
                float moveDirection = Mathf.Sign(directionX); // Returns 1 for right, -1 for left
                _currentFacingDirection = moveDirection > 0 ? Vector2.right : Vector2.left;
                _rb.linearVelocity = new Vector2(moveDirection * moveSpeed, _rb.linearVelocity.y);
            }
            else
            {
                _isWaiting = true;
            }
        }
        
        public bool CanSeeTarget() //this used to take in a bool for whether the target is trans
        {
            //if (isTargetTrans || _playerTarget == null) return false;
            if (_playerTarget == null) return false;

            float distanceToTarget = Vector2.Distance(transform.position, _playerTarget.position);

            if (distanceToTarget <= viewDistance)
            {
                Vector2 directionToTarget = ((Vector2)_playerTarget.position - (Vector2)transform.position).normalized;
                float angle = Vector2.Angle(_currentFacingDirection, directionToTarget);
                
                if (angle <= viewAngle / 2f)
                {
                    Debug.Log("Target is within vision cone! Checking line of sight...");
                    return true; 
                }
            }
            return false;
        }

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
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector3 startPos = transform.position;
            Vector3 facing3D = new Vector3(_currentFacingDirection.x, _currentFacingDirection.y, 0);
            if (facing3D == Vector3.zero) facing3D = Vector3.right;
            Vector3 rightLimit = Quaternion.Euler(0, 0, viewAngle / 2f) * facing3D;
            Vector3 leftLimit = Quaternion.Euler(0, 0, -viewAngle / 2f) * facing3D;
            Gizmos.DrawLine(startPos, startPos + rightLimit * viewDistance);
            Gizmos.DrawLine(startPos, startPos + leftLimit * viewDistance);
        }
    }
}