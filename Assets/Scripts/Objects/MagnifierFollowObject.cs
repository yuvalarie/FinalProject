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

        private Vector3 _offset1;
        private Vector3 _offset2;

        public float AnimationDuration => animationDuration;

        private void Start()
        {
            if (magnifyingGlassCanvas != null)
            {
                _offset1 = magnifyingGlassCanvas.position - transform.position;
            }

            if (magnifyingGlass2 != null && magnifyingGlassCanvas2 != null)
            {
                _offset2 = magnifyingGlassCanvas2.position - magnifyingGlass2.position;
            }

            _topGlassStartPosition = transform.position;

            if (magnifyingGlass2 != null)
            {
                _bottomGlassStartPosition = magnifyingGlass2.position;
            }
        }

        private void LateUpdate()
        {
            if (magnifyingGlassCanvas != null)
            {
                magnifyingGlassCanvas.position = new Vector3(
                    transform.position.x + _offset1.x,
                    transform.position.y + _offset1.y,
                    transform.position.z
                );
            }

            if (magnifyingGlass2 != null && magnifyingGlassCanvas2 != null)
            {
                magnifyingGlassCanvas2.position = new Vector3(
                    magnifyingGlass2.position.x + _offset2.x,
                    magnifyingGlass2.position.y + _offset2.y,
                    magnifyingGlassCanvas2.position.z
                );
            }
        }

        public void FrameTransition()
        {
            if (magnifyingGlass2 == null || topGlassExitPoint == null || bottomGlassTargetPoint == null)
            {
                return;
            }

            Sequence transitionSequence = DOTween.Sequence();

            transitionSequence.Append(transform.DOMove(topGlassExitPoint.position, animationDuration).SetEase(Ease.InOutQuad));
            transitionSequence.Join(magnifyingGlass2.DOMove(bottomGlassTargetPoint.position, animationDuration).SetEase(Ease.InOutQuad));
            transitionSequence.AppendCallback(() => _isOn = false);
        }

        public void BackTransition()
        {
            if (magnifyingGlass2 == null)
            {
                return;
            }

            Sequence transitionSequence = DOTween.Sequence();

            transitionSequence.Append(transform.DOMove(_topGlassStartPosition, animationDuration).SetEase(Ease.InOutQuad));
            transitionSequence.Join(magnifyingGlass2.DOMove(_bottomGlassStartPosition, animationDuration).SetEase(Ease.InOutQuad));
            transitionSequence.AppendCallback(() => _isOn = true);
        }
    }
}
