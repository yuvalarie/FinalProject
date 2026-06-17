using System.Collections;
using Objects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerControllerMiniGame2Hand : PlayerControllerBase
    {
        [Header("Hand Setup")]
        // The hand GameObject — pivot should be at the elbow
        [SerializeField] private Transform handPivot;
        [SerializeField] private float restingAngle = 0f;
        [SerializeField] private float chooseAngle = 60f;
        [SerializeField] private float discardAngle = -60f;
        [SerializeField] private float gizmoRadius = 1.5f;

        [Header("Movement")]
        [SerializeField] private float totalMovementDuration = 1.2f;
        [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("References")]
        [SerializeField] private FriendSpawner friendSpawner;

        private bool _isMoving;
        private bool _swipeLocked = false;

        
        protected override void Start()
        {
            base.Start();
            // Set hand to resting position on enable
            if (handPivot != null)
                handPivot.localRotation = Quaternion.Euler(0f, 0f, restingAngle);
        }
        public void EnableSwiping() => _swipeLocked = false;
        public void DisableSwiping() => _swipeLocked = true;
        

        protected override void HandleMovement()
        {
            if (_isMoving || _swipeLocked) return;

            if (MoveInput.x > 0.5f)
            {
                // Z rotation is inverted relative to gizmos — negate the angle
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
            if (_swipeLocked) return;
            friendSpawner.StartSpawning();
        }

        private IEnumerator SwipeRoutine(float targetAngle, System.Action onReached)
        {
            _isMoving = true;

            float halfDuration = totalMovementDuration * 0.5f;

            // Swing to target
            yield return RotateHand(restingAngle, targetAngle, halfDuration);

            // Trigger the swipe action at the peak of the swing
            onReached?.Invoke();

            // Return to rest
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
                handPivot.localRotation = Quaternion.Euler(0f, 0f, Mathf.LerpAngle(fromAngle, toAngle, easedT));
                yield return null;
            }

            handPivot.localRotation = Quaternion.Euler(0f, 0f, toAngle);
        }

        private void OnChooseReached()
        {
            // Right swipe — spawn the current friend into their assigned frame
            friendSpawner.SpawnFriend();
        }

        private void OnDiscardReached()
        {
            // Left swipe — send the current friend to the hell portal
            friendSpawner.DiscardFriend();
        }

        private void OnDrawGizmos()
        {
            if (handPivot == null) return;

            Vector3 center = handPivot.position;

            Gizmos.color = Color.blue;
            DrawPointOnCircle(center, gizmoRadius, restingAngle);
            Gizmos.color = Color.green;
            DrawPointOnCircle(center, gizmoRadius, chooseAngle);
            Gizmos.color = Color.red;
            DrawPointOnCircle(center, gizmoRadius, discardAngle);
        }

        private void DrawPointOnCircle(Vector3 center, float radius, float angleDegrees)
        {
            float rad = angleDegrees * Mathf.Deg2Rad;
            Vector3 point = center + new Vector3(Mathf.Sin(rad), Mathf.Cos(rad), 0f) * radius;
            Gizmos.DrawWireSphere(point, 0.08f);
        }
    }
}
