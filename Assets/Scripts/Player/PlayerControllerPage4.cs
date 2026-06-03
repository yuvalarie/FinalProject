using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerControllerPage4 : PlayerControllerBase
    {
        private static readonly int Play = Animator.StringToHash("Play");
        private static readonly int Pat = Animator.StringToHash("Pat");

        [Header("Frame2")]
        [SerializeField] private Collider2D frame2EnterCollider;
        [SerializeField] private Animator handAnimator;
        [SerializeField] private Animator pillowAnimator;
        
        [Header("Frame3")]
        [SerializeField] private Collider2D frame3EnterCollider;
        [SerializeField] private Animator hand2Animator;
        [SerializeField] private Collider2D frame3ExitCollider;

        [Header("Frame4")] 
        [SerializeField] private Transform frame4StartPosition;
        [SerializeField] private Collider2D frame4EnterCollider;
        [SerializeField] private Animator hand3Animator;
        [SerializeField] private Animator blanketAnimator;
        
        [Header("Frame5")]
        [SerializeField] private Collider2D frame5EnterCollider;
        [SerializeField] private Animator feetAnimator;
        
        [Header("Frame6")]
        [SerializeField] private Collider2D frame6EnterCollider;
        [SerializeField] private Animator backAnimator;
        [SerializeField] private Animator smallMirrorAnimator;
        [SerializeField] private Animator bigMirrorAnimator;
        [SerializeField] private Collider2D frame6ExitCollider;
        
        [Header("Frame7")]
        [SerializeField] private Transform frame7StartPosition;
        [SerializeField] private Collider2D frame7EnterCollider;
        [SerializeField] private Animator shadowAnimator;
        [SerializeField] private GameObject shadowObject;
        [SerializeField] private Collider2D frame7ExitCollider;
        
        [Header("Frame8")]
        [SerializeField] private Collider2D frame8EnterCollider;
        [SerializeField] private Animator saltAnimator;
        
        [Header("Frame9")]
        [SerializeField] private Collider2D frame9EnterCollider;
        [SerializeField] private Vector3 frame9Size;
        
        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other == frame2EnterCollider)
            {
                Frame2Sequence();
            }
            else if (other == frame3EnterCollider)
            {
                Frame3Sequence();
            }
            else if (other == frame3ExitCollider)
            {
                transform.position = frame4StartPosition.position;
            }
            else if (other == frame4EnterCollider)
            {
                Frame4Sequence();
            }
            else if (other == frame5EnterCollider)
            {
                Frame5Sequence();
            }
            else if (other == frame6EnterCollider)
            {
                Frame6Sequence();
            }
            else if (other == frame6ExitCollider)
            {
                transform.position = frame7StartPosition.position;
            }
            else if (other == frame7EnterCollider)
            {
                Frame7Sequence();
            }
            else if (other == frame7ExitCollider)
            {
                shadowObject.transform.SetParent(null);
                _spriteRenderer.enabled = true;
            }
            else if (other == frame8EnterCollider)
            {
                Frame8Sequence();
            }
            else if (other == frame9EnterCollider)
            {
                Frame9Sequence();
            }
        }
        
        private void Frame2Sequence()
        {
            handAnimator.SetTrigger(Play);
        }

        public void PlayFrame2SecondAnimation()
        {
            pillowAnimator.SetTrigger(Play);
        }
        
        private void Frame3Sequence()
        {
            hand2Animator.SetTrigger(Play);
        }
        
        public void PlayFrame3SecondAnimation()
        {
            hand2Animator.SetTrigger(Pat);
        }
        
        private void Frame4Sequence()
        {
            hand3Animator.SetTrigger(Play);
        }
        
        public void PlayFrame4SecondAnimation()
        {
            blanketAnimator.SetTrigger(Play);
        }
        
        private void Frame5Sequence()
        {
            feetAnimator.SetTrigger(Play);
        }
        
        private void Frame6Sequence()
        {
            backAnimator.SetTrigger(Play);
            smallMirrorAnimator.SetTrigger(Play);
            bigMirrorAnimator.SetTrigger(Play);
        }
        
        private void Frame7Sequence()
        {
            _spriteRenderer.enabled = false;
            shadowObject.transform.SetParent(gameObject.transform);
            shadowAnimator.SetTrigger(Play);
        }
        
        private void Frame8Sequence()
        {
            saltAnimator.SetTrigger(Play);
        }
        
        private void Frame9Sequence()
        {
            transform.localScale = frame9Size;
        }
    }
}