using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    public enum Direction
    {
        Left,
        Right
    }
    [RequireComponent(typeof(Collider2D))]
    public class Portal : MonoBehaviour
    {
        [SerializeField] private Portal connectedPortal;
        // [SerializeField] private float teleportCooldown = 0.01f;
        // [SerializeField] private Transform topMiniBossPosition;
        // [SerializeField] private Transform bottomMiniBossPosition;

        [SerializeField] private Direction facingDirection;

        // private const float TeleportCooldown = 1f;
        private readonly HashSet<Transform> _recentlyTeleported = new HashSet<Transform>();
        private Vector2 _positionToAddTeleportation;

        private void Start()
        {
            _positionToAddTeleportation = transform.position - connectedPortal.transform.position;
            if (facingDirection == Direction.Right)
            {
                _positionToAddTeleportation.x += 0.5f;
            }
            else if (facingDirection == Direction.Left)
            {
                _positionToAddTeleportation.x -= 0.5f;
            }
            else
            {
                Debug.LogError("Portal facing direction is not set to Left or Right.");
            }
            // Debug.Log("Portal facind direction: " + facingDirection + "calculated position to add: " + _positionToAddTeleportation);
            // facingDirection = facingDirection.normalized;
        }
        
        // private void Update()
        // {
        //     Color lineColor;
        //     switch (facingDirection)
        //     {
        //         case Direction.Right:
        //             lineColor = Color.red;
        //             break;
        //         case Direction.Left:
        //             lineColor = Color.blue;
        //             break;
        //         default:
        //             lineColor = Color.magenta;
        //             break;
        //     }
        //
        //     Vector3 linePosition = transform.position + (Vector3)_positionToAddTeleportation;
        //     Vector3 start = new Vector3(linePosition.x, 4.5f, 0f);
        //     Vector3 end = new Vector3(linePosition.x, -4.5f, 0f);
        //     Debug.DrawLine(start, end, lineColor);
        // }
        
        public void SetConnectedPortal(Portal portal)
        {
            connectedPortal = portal;
            _positionToAddTeleportation = transform.position - connectedPortal.transform.position;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var rb = other.attachedRigidbody;
            var transformToCheck = rb ? rb.transform : other.transform;
            if (_recentlyTeleported.Contains(transformToCheck)) return;
            Teleport(transformToCheck);
        }
        private void Teleport(Transform objectToTeleport)
        {
            if (connectedPortal == null || objectToTeleport == null) return;
            StartCoroutine(TeleportRoutine(objectToTeleport));
        }

        private IEnumerator TeleportRoutine(Transform objectToTeleport)
        {
            yield return new WaitForEndOfFrame();
            connectedPortal.ReceiveTeleportedObject(objectToTeleport);
        }

        private void ReceiveTeleportedObject(Transform objectToReceive)
        {
            _recentlyTeleported.Add(objectToReceive);
            objectToReceive.position = new Vector2(objectToReceive.position.x, objectToReceive.position.y) + _positionToAddTeleportation;
            // var rb = objectToReceive.GetComponent<Rigidbody2D>();
            // if (rb != null)
            // {
            //     rb.linearVelocity *= facingDirection.ToVector2();
            // }
            // // StartCoroutine(RemoveFromRecentlyTeleported(objectToReceive));
        }

        // private IEnumerator RemoveFromRecentlyTeleported(Transform objectToReceive)
        // {
        //     yield return new WaitForSeconds(teleportCooldown);
        //     _recentlyTeleported.Remove(objectToReceive);
        // }

        private void OnTriggerExit2D(Collider2D other)
        {
            var rb = other.attachedRigidbody;
            var transformToCheck = rb ? rb.transform : other.transform;
            _recentlyTeleported.Remove(transformToCheck);
        }

        // private void OnDrawGizmos()
        // {
        //     Gizmos.color = Color.cyan;
        //     Vector3 start = transform.position;
        //     Vector3 end = start + (Vector3)facingDirection.ToVector2().normalized * 1.5f; // 1.5 units long
        //     Gizmos.DrawLine(start, end);
        //     Gizmos.DrawSphere(end, 0.1f); // Optional: draw a sphere at the end
        // }
    }
}
