using UnityEngine;

namespace Objects
{
    public class RoundedTrailFollower : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private bool detachFromParentAtRuntime = true;
        [SerializeField, Min(0f)] private float smoothTime = 0f;
        [SerializeField] private float maxSpeed = 300f;
        [SerializeField] private float maxLagDistance = 0f;
        [SerializeField] private float teleportDistance = 1.5f;
        [SerializeField] private bool clearTrailsOnTeleport = true;

        private TrailRenderer[] trails;
        private Vector3 localOffset;
        private Vector3 velocity;

        private void Awake()
        {
            if (target == null)
            {
                target = transform.parent;
            }

            if (target != null)
            {
                localOffset = target.InverseTransformPoint(transform.position);
            }

            trails = GetComponentsInChildren<TrailRenderer>(true);

            if (detachFromParentAtRuntime)
            {
                transform.SetParent(null, true);
            }
        }

        private void OnEnable()
        {
            SnapToTarget(true);
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            Vector3 targetPosition = target.TransformPoint(localOffset);
            float distance = Vector3.Distance(transform.position, targetPosition);

            if (teleportDistance > 0f && distance > teleportDistance)
            {
                SnapToTarget(clearTrailsOnTeleport);
                return;
            }

            Vector3 smoothedPosition = smoothTime > 0f
                ? Vector3.SmoothDamp(
                    transform.position,
                    targetPosition,
                    ref velocity,
                    smoothTime,
                    maxSpeed,
                    Time.deltaTime)
                : targetPosition;

            if (maxLagDistance > 0f)
            {
                Vector3 lag = smoothedPosition - targetPosition;
                if (lag.sqrMagnitude > maxLagDistance * maxLagDistance)
                {
                    smoothedPosition = targetPosition + lag.normalized * maxLagDistance;
                    velocity = Vector3.ProjectOnPlane(velocity, lag.normalized);
                }
            }

            transform.position = smoothedPosition;
            transform.rotation = Quaternion.identity;
        }

        public void SnapToTarget(bool clearTrail)
        {
            if (target == null)
            {
                return;
            }

            velocity = Vector3.zero;
            transform.position = target.TransformPoint(localOffset);
            transform.rotation = Quaternion.identity;

            if (!clearTrail || trails == null)
            {
                return;
            }

            foreach (TrailRenderer trail in trails)
            {
                if (trail != null)
                {
                    trail.Clear();
                }
            }
        }
    }
}
