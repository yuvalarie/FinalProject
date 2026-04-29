using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Npc
{
    public class WatchingNpcController : MonoBehaviour
    {
        [SerializeField, Tooltip("How far the character turns from its starting rotation.")]
        private float turnAngle = 180f;

        [SerializeField, Tooltip("How long they stay looking behind them.")]
        private float watchDuration = 2f;

        [SerializeField, Tooltip("Time spent facing forward before turning again.")]
        private float waitTime = 5f;

        [SerializeField, Tooltip("The point from which the line of sight is calculated (the head).")]
        private Transform eyes;

        [SerializeField, Tooltip("The layer the active character is assigned to.")]
        private LayerMask targetLayer;

        [SerializeField, Tooltip("How far the watcher can see.")]
        private float viewDistance = 20f;

        [SerializeField, Tooltip("The width of the vision cone (angle from center).")]
        private float viewAngle = 60f;

        private Quaternion _startRotation;
        private Quaternion _backRotation;
        private bool _isLookingBack = false;

        void Start()
        {
            _startRotation = transform.rotation;
            _backRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, turnAngle, 0));
            StartCoroutine(WatchRoutine());
        }

        void Update()
        {
            if (_isLookingBack)
            {
                CheckForTargets();
            }
        }

        private void CheckForTargets()
        {
            Collider2D targetCollider = Physics2D.OverlapCircle(eyes.position, viewDistance, targetLayer);
            if (targetCollider == null) return;

            Transform target = targetCollider.transform;
            Vector2 directionToTarget = (target.position - eyes.position).normalized;
            Debug.DrawRay(eyes.position, directionToTarget * viewDistance, Color.yellow);
            if (Vector2.Angle(eyes.right, directionToTarget) < viewAngle / 2f)
            {
                RaycastHit2D hit = Physics2D.Raycast(eyes.position, directionToTarget, viewDistance, targetLayer);
                if (hit.collider != null && hit.collider == targetCollider)
                {
                    var player = hit.collider.GetComponent<PlayerControllerBase>();
            
                    if (player != null)
                    {
                        if (player.IsTrans) return;
                        Debug.Log($"CAUGHT! Spotted {target.name} moving or holding an item in the vision cone.");
                        Debug.DrawRay(eyes.position, directionToTarget * hit.distance, Color.red);
                    }
                }
            }
        }

        private IEnumerator WatchRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(waitTime);
                transform.rotation = _backRotation;
                _isLookingBack = true;
            
                yield return new WaitForSeconds(watchDuration);
                transform.rotation = _startRotation;
                _isLookingBack = false;
            }
        }

        private void OnDrawGizmos()
        {
            if (eyes == null) return;
        
            // Visualize the view distance
            Gizmos.color = _isLookingBack ? Color.red : Color.green;
            Gizmos.DrawWireSphere(eyes.position, viewDistance);

            // Visualize the vision cone boundaries
            Vector3 leftBoundary = Quaternion.AngleAxis(-viewAngle / 2f, Vector3.forward) * eyes.right;
            Vector3 rightBoundary = Quaternion.AngleAxis(viewAngle / 2f, Vector3.forward) * eyes.right;
        
            Gizmos.DrawRay(eyes.position, leftBoundary * viewDistance);
            Gizmos.DrawRay(eyes.position, rightBoundary * viewDistance);
        }
    }
}
