using System;
using UnityEngine;
using DG.Tweening;
using Player;

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
            _bottomGlassStartPosition = magnifyingGlass2.position;
        }

        private void LateUpdate()
        {
            // Top frame logic
            magnifyingGlassCanvas.position = new Vector3(
                transform.position.x + _offset1.x, 
                transform.position.y + _offset1.y, 
                transform.position.z
            );
            // Bottom frame logic
            magnifyingGlassCanvas2.position = new Vector3(
                magnifyingGlass2.position.x + _offset2.x, 
                magnifyingGlass2.position.y + _offset2.y, 
                magnifyingGlassCanvas2.position.z
            );
        }

        public void FrameTransition()
        {
            // Create a DOTween Sequence to queue up animations back-to-back
            Sequence transitionSequence = DOTween.Sequence();

            // 1. Move the top glass down to its exit point (easing makes it start slow and speed up)
            transitionSequence.Append(transform.DOMove(topGlassExitPoint.position, animationDuration).SetEase(Ease.InOutQuad));

            // 3. Move the bottom glass from its starting position into view (easing makes it smoothly settle into place)
            transitionSequence.Join(magnifyingGlass2.DOMove(bottomGlassTargetPoint.position, animationDuration).SetEase(Ease.InOutQuad));
            
            // 2. Swap the _isOn boolean the exact millisecond the top glass disappears
            transitionSequence.AppendCallback(() => 
            {
                _isOn = false;
                PlayerControllerPage6.TriggerSequenceComplete();
            });
        }
        
        public void BackTransition()
        {
            // Create a DOTween Sequence to queue up animations back-to-back
            Sequence transitionSequence = DOTween.Sequence();

            // 1. Move the top glass down to its exit point (easing makes it start slow and speed up)
            transitionSequence.Append(transform.DOMove(_topGlassStartPosition, animationDuration).SetEase(Ease.InOutQuad));

            // 3. Move the bottom glass from its starting position into view (easing makes it smoothly settle into place)
            transitionSequence.Join(magnifyingGlass2.DOMove(_bottomGlassStartPosition, animationDuration).SetEase(Ease.InOutQuad));
            
            // 2. Swap the _isOn boolean the exact millisecond the top glass disappears
            transitionSequence.AppendCallback(() => 
            {
                _isOn = true;
                PlayerControllerPage6.TriggerBackSequenceComplete();
            });

        }
    }
}