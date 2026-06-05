using System;
using System.Collections;
using Objects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public enum TinderSwipeDirection
    {
        Left, // like tinder, swipe left 
        Right, 
        Middle
    }
    
    public class PlayerControllerMiniGame4New : PlayerControllerBase
    {
        [Header("Hand Setup")]
        [SerializeField] private Transform handPivot; // the whole hand object, pivot at elbow
        [SerializeField] private float radius = 1.5f;
        [SerializeField] private float restingAngle = 0f;   // 12 o'clock
        [SerializeField] private float chooseAngle = 60f;    // configurable
        [SerializeField] private float discardAngle = -60f;   // configurable

        [Header("Movement")]
        [SerializeField] private float totalMovementDuration = 1.2f;
        [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("References")]
        [SerializeField] private FriendSpawner friendSpawner;
        private bool _isMoving;

        private void Start()
        {
            if (handPivot != null)
            {
                handPivot.localRotation = Quaternion.Euler(0f, 0f, restingAngle);
            }
        }

        protected override void HandleMovement()
        {
            if (_isMoving) return;

            if (MoveInput.x > 0.5f)
            {
                // the rotation on the z axis is not like the gizmos. here moving to the positive is to move to the left, and negative is to move to the right.
                // so we need to call on the negative of the angles we choose. 
                MoveInput.x = 0f;
                StartCoroutine(SwipeRoutine(-chooseAngle, OnChooseReached));
            }
            else if (MoveInput.x < -0.5f)
            {
                MoveInput.x = 0f;
                StartCoroutine(SwipeRoutine(-discardAngle, OnDiscardReached));
            }
        }

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            // Not used in this mini-game
        }

        private IEnumerator SwipeRoutine(float targetAngle, System.Action onReached)
        {
            _isMoving = true;

            float halfDuration = totalMovementDuration * 0.5f;

            yield return RotateHand(restingAngle, targetAngle, halfDuration);

            onReached?.Invoke();

            yield return RotateHand(targetAngle, restingAngle, halfDuration);

            _isMoving = false;
        }

        private IEnumerator RotateHand(float fromAngle, float toAngle, float duration)
        {
            if (handPivot == null) yield break;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float easedT = movementCurve.Evaluate(t);

                float currentAngle = Mathf.LerpAngle(fromAngle, toAngle, easedT);
                handPivot.localRotation = Quaternion.Euler(0f, 0f, currentAngle);

                yield return null;
            }

            handPivot.localRotation = Quaternion.Euler(0f, 0f, toAngle);
        }

        private void OnChooseReached()
        {
            Debug.Log("Choose action reached");
            // your choose logic here
            friendSpawner.SpawnFriend();
        }

        private void OnDiscardReached()
        {
            Debug.Log("Discard action reached");
            // your discard logic here
            friendSpawner.ShowNextFriend();
        }

        private void OnDrawGizmos()
        {
            if (handPivot == null) return;

            Vector3 center = handPivot.position;

            // Draw full circle in yellow
            Gizmos.color = Color.yellow;
            DrawCircleGizmo(center, radius, 64);

            // Resting position in blue
            Gizmos.color = Color.blue;
            DrawPointOnCircle(center, radius, restingAngle);

            // Choose position in green
            Gizmos.color = Color.green;
            DrawPointOnCircle(center, radius, chooseAngle);

            // Discard position in red
            Gizmos.color = Color.red;
            DrawPointOnCircle(center, radius, discardAngle);
        }

        private void DrawCircleGizmo(Vector3 center, float gizRadius, int segments)
        {
            float angleStep = 360f / segments;

            // 0° = up, so start at (0, +radius)
            Vector3 previousPoint = center + new Vector3(0f, 1f, 0f) * gizRadius;

            for (int i = 1; i <= segments; i++)
            {
                float angleDeg = i * angleStep;
                float rad = angleDeg * Mathf.Deg2Rad;

                // 0° = up, 90° = right, 180° = down, 270° = left
                Vector3 point = center + new Vector3(Mathf.Sin(rad), Mathf.Cos(rad), 0f) * gizRadius;
                Gizmos.DrawLine(previousPoint, point);
                previousPoint = point;
            }
        }

        private void DrawPointOnCircle(Vector3 center, float gizRadius, float angleDegrees)
        {
            float rad = angleDegrees * Mathf.Deg2Rad;

            // 0° = up
            Vector3 point = center + new Vector3(Mathf.Sin(rad), Mathf.Cos(rad), 0f) * gizRadius;
            Gizmos.DrawWireSphere(point, 0.08f);
        }
        
        // [Header("Positions for hand movement")]
        // [SerializeField] private Transform restPosition;
        // [SerializeField] private Transform elbowPivot;
        // [SerializeField] private Transform palmEndpoint;
        // [SerializeField] private Transform leftButton;
        // [SerializeField] private Transform rightButton;
        //
        // [Header("Animation")]
        // [SerializeField] private float totalMovementTime = 1.2f;
        // [SerializeField] private AnimationCurve easeOut = AnimationCurve.EaseInOut(0, 0, 1, 1);
        // [SerializeField] private AnimationCurve easeBack = AnimationCurve.EaseInOut(0, 0, 1, 1);
        //
        // [Header("References")]
        // // [SerializeField] private GameObject hand;
        // [SerializeField] private FriendSpawner friendSpawner;
        // private TinderSwipeDirection _currentSwipeDirection = TinderSwipeDirection.Middle;
        // private bool _isMoving = false;
        // private Vector2 _restPalmPos;
        // private float _halfDuration;
        //
        // private void Start()
        // {
        //     _restPalmPos = restPosition.position;
        //     _halfDuration = totalMovementTime / 2f;
        //     palmEndpoint.position = _restPalmPos;
        // }
        //
        // protected override void OnInteraction(InputAction.CallbackContext context)
        // {
        //     // if(_currentSwipeDirection == TinderSwipeDirection.Left) friendSpawner.ShowNextFriend();
        //     // else friendSpawner.SpawnFriend();
        // }
        //
        // private void OnButtonReached()
        // {
        //     if (_currentSwipeDirection == TinderSwipeDirection.Left) friendSpawner.ShowNextFriend();
        //     else friendSpawner.SpawnFriend();
        //     _currentSwipeDirection = TinderSwipeDirection.Middle; // reset to middle to prevent multiple triggers
        // }
        //
        // protected override void HandleMovement()
        // {
        //     // if (MoveInput.x > 0.1f)
        //     // {
        //     //     _currentSwipeDirection = TinderSwipeDirection.Right;
        //     //     hand.transform.position = rightButton.position;
        //     // }
        //     // else if (MoveInput.x < -0.1f)
        //     // {
        //     //     _currentSwipeDirection = TinderSwipeDirection.Left;
        //     //     hand.transform.position = leftButton.position;
        //     // }
        //     
        //     if(_isMoving) return;
        //     
        //     if (MoveInput.x > 0.1f)
        //     {
        //         _currentSwipeDirection = TinderSwipeDirection.Right;
        //         MoveInput.x = 0f; // prevent continuous movement input while animating
        //         StartCoroutine(SwipeAndReturn(rightButton.position));
        //     }
        //     else if (MoveInput.x < -0.1f)
        //     {
        //         _currentSwipeDirection = TinderSwipeDirection.Left;
        //         MoveInput.x = 0f; // prevent continuous movement input while animating
        //         StartCoroutine(SwipeAndReturn(leftButton.position));
        //     }
        // }
        //
        // private IEnumerator SwipeAndReturn(Vector2 targetPos)
        // {
        //     _isMoving = true;
        //     yield return ArcMovement(_restPalmPos, targetPos, _halfDuration, easeOut);
        //     OnButtonReached();
        //     yield return ArcMovement(targetPos, _restPalmPos, _halfDuration, easeBack);
        //     _isMoving = false;
        // }
        //
        // private IEnumerator ArcMovement(Vector2 from, Vector2 to, float duration, AnimationCurve curve)
        // {
        //     Vector2 pivot = elbowPivot.position;
        //     
        //     Vector2 fromDir = (from - pivot).normalized;
        //     Vector2 toDir   = (to   - pivot).normalized;
        //     
        //     float fromAngle = Mathf.Atan2(fromDir.y, fromDir.x) * Mathf.Rad2Deg;
        //     float toAngle   = Mathf.Atan2(toDir.y,   toDir.x)   * Mathf.Rad2Deg;
        //     
        //     float angleDelta = Mathf.DeltaAngle(fromAngle, toAngle);
        //     float radius = Vector2.Distance(pivot, from);
        //     
        //     float elapsed = 0f;
        //     while (elapsed < duration)
        //     {
        //         elapsed += Time.deltaTime;
        //         float t = Mathf.Clamp01(elapsed / duration);
        //         float eased = curve.Evaluate(t);
        //         
        //         float currentAngle = fromAngle + angleDelta * eased;
        //         float rad = currentAngle * Mathf.Deg2Rad;
        //         
        //         Vector2 palmPos = pivot + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
        //         palmEndpoint.position = palmPos;
        //         
        //         yield return null;
        //     }
        //     
        //     palmEndpoint.position = to;
        // }
    }
}
