using System;
using DG.Tweening; 
using UnityEngine;
using Random = UnityEngine.Random;

namespace Npc
{
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
        
        [Header("Bounce Settings")]
        [Tooltip("How high the NPC bounces off the ground.")]
        [SerializeField] private float bounceHeight = 0.5f;
        
        // --- NEW VARIABLE ---
        [Tooltip("Exactly how many times the NPC jumps to reach its target.")]
        [SerializeField] private int jumpsPerMove = 3; 

        private int _currentWaypointIndex = 0;
        private Vector2 _currentTargetPosition;
        private Vector2 _currentFacingDirection = Vector2.right;
        
        private Sequence _currentMoveSequence; 

        private void Start()
        {
            PickNextWaypoint();
        }

        private void PickNextWaypoint()
        {
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

            MoveWithBounce();
        }
        
        private void GenerateRandomWanderPoint()
        {
            Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
            randomOffset.y = 0; 

            _currentTargetPosition = (Vector2)transform.position + randomOffset;
        }

        private void MoveWithBounce()
        {
            float directionX = _currentTargetPosition.x - transform.position.x;
            _currentFacingDirection = directionX > 0 ? Vector2.right : Vector2.left;

            float distance = Vector2.Distance(transform.position, _currentTargetPosition);
            float duration = distance / moveSpeed; 
            
            _currentMoveSequence?.Kill();

            _currentMoveSequence = DOTween.Sequence();
            
            // --- UPDATED: Passing the exact number of jumps ---
            _currentMoveSequence.Append(
                // transform.DOJump(_currentTargetPosition, bounceHeight, jumpsPerMove, duration)
                //     .SetEase(Ease.Linear) 
                    transform.DOMove(_currentTargetPosition, duration).SetEase(Ease.Linear)
            );

            _currentMoveSequence.AppendInterval(waitTimeAtPoint);
            
            _currentMoveSequence.OnComplete(PickNextWaypoint);
        }

        public void ClearWaypoints()
        {
            waypoints = Array.Empty<Transform>();
        }

        private void OnDestroy()
        {
            _currentMoveSequence?.Kill();
        }
    }
}