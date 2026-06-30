using UnityEngine;

namespace Objects
{
    public class RoundedTrailFollower : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private bool detachFromParentAtRuntime = true;
        [SerializeField, Min(0.001f)] private float smoothTime = 0.025f;
        [SerializeField] private float maxSpeed = 300f;
        [SerializeField] private float maxLagDistance = 0.06f;
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

            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref velocity,
                smoothTime,
                maxSpeed,
                Time.deltaTime);

            Vector3 lag = transform.position - targetPosition;
            if (maxLagDistance > 0f && lag.magnitude > maxLagDistance)
            {
                transform.position = targetPosition + lag.normalized * maxLagDistance;
                velocity = Vector3.zero;
            }

            transform.rotation = target.rotation;
        }

        public void SnapToTarget(bool clearTrail)
        {
            if (target == null)
            {
                return;
            }

            velocity = Vector3.zero;
            transform.position = target.TransformPoint(localOffset);
            transform.rotation = target.rotation;

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
