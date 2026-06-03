using System;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerControllerPage4 : PlayerControllerBase
    {
        private static readonly int Play = Animator.StringToHash("Play");
        private static readonly int Pat = Animator.StringToHash("Pat");
        
        [Header("Frame1")]
        [SerializeField] private Collider2D frame1ExitCollider;

        [Header("Frame2")]
        [SerializeField] private Collider2D frame2EnterCollider;
        [SerializeField] private Transform frame2StartPosition;
        [SerializeField] private Animator handAnimator;
        [SerializeField] private Animator pillowAnimator;
        
        [Header("Frame3")]
        [SerializeField] private Collider2D frame3EnterCollider;
        [SerializeField] private Animator hand2Animator;
        [SerializeField] private Collider2D frame3ExitCollider;
        [SerializeField] private Transform frame3ExitPosition;

        [Header("Frame4")] 
        [SerializeField] private Transform frame4StartPosition;
        [SerializeField] private Collider2D frame4EnterCollider;
        [SerializeField] private Collider2D frame4ExitCollider;
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
        [SerializeField] private Transform frame6ExitPosition;
        
        [Header("Frame7")]
        [SerializeField] private Transform frame7StartPosition;
        [SerializeField] private Collider2D frame7EnterCollider;
        [SerializeField] private Collider2D frame7EnterRightCollider;
        [SerializeField] private Animator shadowAnimator;
        [SerializeField] private GameObject shadowObject;
        [SerializeField] private Collider2D frame7ExitCollider;
        [SerializeField] private Collider2D frame7ExitLeftCollider;
        
        [Header("Frame8")]
        [SerializeField] private Collider2D frame8EnterCollider;
        [SerializeField] private Animator saltAnimator;
        
        [Header("Frame9")]
        [SerializeField] private Collider2D frame9EnterCollider;
        [SerializeField] private Vector3 frame9Size;
        
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;
        private Vector3 _shadowOriginalPosition;

        protected override void Awake()
        {
            base.Awake();
            _animator = GetComponent<Animator>();
            _animator.SetTrigger(Play);
        }

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
                if (shadowObject != null) _shadowOriginalPosition = shadowObject.transform.position;
        }

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other == frame1ExitCollider)
            {
                transform.position = frame2StartPosition.position;
            }
            else if (other == frame2EnterCollider)
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
            else if (other == frame4ExitCollider)
            {
                transform.position = frame3ExitPosition.position;
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
            else if (other == frame7EnterRightCollider)
            {
                shadowObject.transform.SetParent(gameObject.transform);
                _spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
            }
            else if (other == frame7ExitCollider)
            {
                shadowObject.transform.SetParent(null);
                _spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            }
            else if (other == frame7ExitLeftCollider)
            {
                shadowObject.transform.SetParent(null);
                shadowObject.transform.position = _shadowOriginalPosition;
                _spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
                transform.position = frame6ExitPosition.position;
            }
            else if (other == frame8EnterCollider)
            {
                Frame8Sequence();
            }
            else if (other == frame9EnterCollider)
            {
                Frame9Sequence();
            }
            if (other.CompareTag("End"))
            {
                SceneLoader.Instance?.LoadScene(nextSceneName);
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
            _spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
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