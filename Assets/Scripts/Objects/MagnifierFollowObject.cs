using DG.Tweening;
using UnityEngine;

namespace Objects
{
    public class MagnifierFollowObject : MonoBehaviour
    {
        [Header("UI Canvas Masks")]
        [Tooltip("The UI Canvas mask for the top frame")]
        [SerializeField] private Transform magnifyingGlassCanvas;

        [Tooltip("The UI Canvas mask for the bottom frame")]
        [SerializeField] private Transform magnifyingGlassCanvas2;

        [Header("Scene Objects")]
        [Tooltip("The Scene magnifying glass object for the bottom frame")]
        [SerializeField] private Transform magnifyingGlass2;

        [Tooltip("Drag the invisible boundary/border collider that separates the frames here")]
        [SerializeField] private Collider2D frameBorderCollider;

        [Header("Transition Settings")]
        [Tooltip("Create an empty GameObject where the top glass should slide down to, and drag it here")]
        [SerializeField] private Transform topGlassExitPoint;
        private Vector3 _topGlassStartPosition;

        [Tooltip("Create an empty GameObject where the bottom glass should slide to, and drag it here")]
        [SerializeField] private Transform bottomGlassTargetPoint;
        private Vector3 _bottomGlassStartPosition;

        [Tooltip("How many seconds each movement takes")]
        [SerializeField] private float animationDuration = 0.8f;

        private bool _isOn = true;

        private RectTransform _magnifyingGlassCanvasRect;
        private RectTransform _magnifyingGlassCanvas2Rect;
        private Canvas _magnifyingGlassCanvasRoot;
        private Canvas _magnifyingGlassCanvas2Root;
        private RectTransform _magnifyingGlassCanvasParentRect;
        private RectTransform _magnifyingGlassCanvas2ParentRect;
        private Vector2 _canvasOffset1;
        private Vector2 _canvasOffset2;
        private Sequence _transitionSequence;
        private bool _hasTopCanvasBinding;
        private bool _hasBottomCanvasBinding;
        private bool _reportedMissingReferences;

        public float AnimationDuration => animationDuration;

        private void Start()
        {
            _hasTopCanvasBinding = TryCacheCanvasBinding(
                magnifyingGlassCanvas,
                transform,
                out _magnifyingGlassCanvasRect,
                out _magnifyingGlassCanvasRoot,
                out _magnifyingGlassCanvasParentRect,
                out _canvasOffset1);

            _hasBottomCanvasBinding = TryCacheCanvasBinding(
                magnifyingGlassCanvas2,
                magnifyingGlass2,
                out _magnifyingGlassCanvas2Rect,
                out _magnifyingGlassCanvas2Root,
                out _magnifyingGlassCanvas2ParentRect,
                out _canvasOffset2);

            _topGlassStartPosition = transform.position;

            if (magnifyingGlass2 != null)
            {
                _bottomGlassStartPosition = magnifyingGlass2.position;
            }
        }

        private void LateUpdate()
        {
            UpdateCanvasFollower(
                _hasTopCanvasBinding,
                _magnifyingGlassCanvasRect,
                _magnifyingGlassCanvasRoot,
                _magnifyingGlassCanvasParentRect,
                transform,
                _canvasOffset1);

            UpdateCanvasFollower(
                _hasBottomCanvasBinding,
                _magnifyingGlassCanvas2Rect,
                _magnifyingGlassCanvas2Root,
                _magnifyingGlassCanvas2ParentRect,
                magnifyingGlass2,
                _canvasOffset2);
        }

        public void FrameTransition()
        {
            if (!CanRunTransition(topGlassExitPoint, bottomGlassTargetPoint))
            {
                return;
            }

            RestartTransitionSequence();

            _transitionSequence.Append(transform.DOMove(topGlassExitPoint.position, animationDuration).SetEase(Ease.InOutQuad));
            _transitionSequence.Join(magnifyingGlass2.DOMove(bottomGlassTargetPoint.position, animationDuration).SetEase(Ease.InOutQuad));
            _transitionSequence.AppendCallback(() => _isOn = false);
        }

        public void BackTransition()
        {
            if (magnifyingGlass2 == null)
            {
                ReportMissingReferences();
                return;
            }

            RestartTransitionSequence();

            _transitionSequence.Append(transform.DOMove(_topGlassStartPosition, animationDuration).SetEase(Ease.InOutQuad));
            _transitionSequence.Join(magnifyingGlass2.DOMove(_bottomGlassStartPosition, animationDuration).SetEase(Ease.InOutQuad));
            _transitionSequence.AppendCallback(() => _isOn = true);
        }

        private bool TryCacheCanvasBinding(
            Transform canvasFollower,
            Transform worldTarget,
            out RectTransform followerRect,
            out Canvas canvasRoot,
            out RectTransform parentRect,
            out Vector2 offset)
        {
            followerRect = canvasFollower as RectTransform;
            canvasRoot = followerRect != null ? followerRect.GetComponentInParent<Canvas>() : null;
            parentRect = followerRect != null ? followerRect.parent as RectTransform : null;
            offset = Vector2.zero;

            if (followerRect == null || canvasRoot == null || parentRect == null || worldTarget == null)
            {
                ReportMissingReferences();
                return false;
            }

            UnityEngine.Camera canvasCamera = GetCanvasCamera(canvasRoot);
            Vector2 followerScreenPosition = RectTransformUtility.WorldToScreenPoint(canvasCamera, followerRect.position);
            Vector2 targetScreenPosition = WorldToScreenPoint(canvasRoot, worldTarget.position);
            offset = followerScreenPosition - targetScreenPosition;
            return true;
        }

        private void UpdateCanvasFollower(
            bool hasBinding,
            RectTransform followerRect,
            Canvas canvasRoot,
            RectTransform parentRect,
            Transform worldTarget,
            Vector2 offset)
        {
            if (!hasBinding || followerRect == null || canvasRoot == null || parentRect == null || worldTarget == null)
            {
                return;
            }

            UnityEngine.Camera canvasCamera = GetCanvasCamera(canvasRoot);
            Vector2 targetScreenPosition = WorldToScreenPoint(canvasRoot, worldTarget.position) + offset;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(parentRect, targetScreenPosition, canvasCamera, out Vector3 targetWorldPosition))
            {
                followerRect.position = targetWorldPosition;
            }
        }

        private static UnityEngine.Camera GetCanvasCamera(Canvas canvasRoot)
        {
            return canvasRoot.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvasRoot.worldCamera;
        }

        private static Vector2 WorldToScreenPoint(Canvas canvasRoot, Vector3 worldPosition)
        {
            UnityEngine.Camera canvasCamera = GetCanvasCamera(canvasRoot);
            return canvasCamera != null ? canvasCamera.WorldToScreenPoint(worldPosition) : (Vector2)worldPosition;
        }

        private bool CanRunTransition(Transform topTarget, Transform bottomTarget)
        {
            if (magnifyingGlass2 != null && topTarget != null && bottomTarget != null)
            {
                return true;
            }

            ReportMissingReferences();
            return false;
        }

        private void RestartTransitionSequence()
        {
            _transitionSequence?.Kill();
            _transitionSequence = DOTween.Sequence();
        }

        private void ReportMissingReferences()
        {
            if (_reportedMissingReferences)
            {
                return;
            }

            _reportedMissingReferences = true;
            Debug.LogWarning($"{nameof(MagnifierFollowObject)} on {name} has missing or invalid references. Check the magnifier world objects, UI mask RectTransforms, and target points.", this);
        }

        private void OnDisable()
        {
            _transitionSequence?.Kill();
            _transitionSequence = null;
        }
    }
}
