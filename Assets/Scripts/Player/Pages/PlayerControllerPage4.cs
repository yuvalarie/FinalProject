using System;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerControllerPage4 : PlayerControllerBase
    {
        private static readonly int Play = Animator.StringToHash("Play");
        private static readonly int Pat = Animator.StringToHash("Pat");
        
        [Header("Frame1")]
        [SerializeField] private Collider2D frame1ExitCollider;
        [SerializeField] private GameObject sleepingHelda1;
        [SerializeField] private GameObject sleepingHelda2;
        [SerializeField] private GameObject standingHelda;
        [SerializeField] private GameObject textBubble1;

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
        [SerializeField] private GameObject textBubble2;
        
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
        //[SerializeField] private Vector3 frame8Size;
        [SerializeField] private SizeSettings frame8Size;
        [SerializeField] private GameObject textBubble3;
        [SerializeField] private GameObject textBubble4;
        [SerializeField] private GameObject textBubble5;
        
        [Header("Frame9")]
        [SerializeField] private Collider2D frame9EnterCollider;
        [SerializeField] private Collider2D frame9ExitCollider;
        //[SerializeField] private Vector3 frame9Size;
        [SerializeField] private SizeSettings frame9Size;
        
        private Vector3 _shadowOriginalPosition;

        protected override void Start()
        {
            base.Start();
            if (shadowObject != null) _shadowOriginalPosition = shadowObject.transform.position;
        }

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            if (other == frame1ExitCollider)
            {
                transform.position = frame2StartPosition.position;
                sleepingHelda1.SetActive(false);
                sleepingHelda2.SetActive(false);
                standingHelda.SetActive(true);
            }
            else if (other == frame2EnterCollider)
            {
                textBubble1.SetActive(true);
                Frame2Sequence();
                frame2EnterCollider.enabled = false;
            }
            else if (other == frame3EnterCollider)
            {
                Frame3Sequence();
                frame3EnterCollider.enabled = false;
            }
            else if (other == frame3ExitCollider)
            {
                transform.position = frame4StartPosition.position;
            }
            else if (other == frame4EnterCollider)
            {
                Frame4Sequence();
                frame4EnterCollider.enabled = false;
            }
            else if (other == frame4ExitCollider)
            {
                transform.position = frame3ExitPosition.position;
            }
            else if (other == frame5EnterCollider)
            {
                Frame5Sequence();
                frame5EnterCollider.enabled = false;
            }
            else if (other == frame6EnterCollider)
            {
                Frame6Sequence();
                frame6EnterCollider.enabled = false;
            }
            else if (other == frame6ExitCollider)
            {
                transform.position = frame7StartPosition.position;
            }
            else if (other == frame7EnterCollider)
            {
                Frame7Sequence();
                frame7EnterCollider.enabled = false;
            }
            else if (other == frame7EnterRightCollider)
            {
                shadowObject.transform.SetParent(gameObject.transform);
                SpriteRenderer.color = new Color(1f, 1f, 1f, 0f);
            }
            else if (other == frame7ExitCollider)
            {
                shadowObject.transform.SetParent(null);
                SpriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            }
            else if (other == frame7ExitLeftCollider)
            {
                shadowObject.transform.SetParent(null);
                shadowObject.transform.position = _shadowOriginalPosition;
                SpriteRenderer.color = new Color(1f, 1f, 1f, 1f);
                transform.position = frame6ExitPosition.position;
            }
            else if (other == frame8EnterCollider)
            {
                Frame8Sequence();
                frame8EnterCollider.enabled = false;
            }
            else if (other == frame9EnterCollider)
            {
                Frame9Sequence();
            }
            else if (other == frame9ExitCollider)
            {
                //transform.localScale = frame8Size;
                CurrentSize = frame8Size;
                SetSize();
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
            SpriteRenderer.color = new Color(1f, 1f, 1f, 0f);
            shadowObject.transform.SetParent(gameObject.transform);
            shadowAnimator.SetTrigger(Play);
        }
        
        private void Frame8Sequence()
        {
            saltAnimator.SetTrigger(Play);
        }
        
        private void Frame9Sequence()
        {
            //transform.localScale = frame9Size;
            CurrentSize = frame9Size;
            SetSize();
        }

        public void SetActiveTextBubble2()
        {
            textBubble2.SetActive(true);
        }
        
        public void SetActiveTextBubble3()
        {
            textBubble3.SetActive(true);
        }
        
        public void SetActiveTextBubble4()
        {
            textBubble4.SetActive(true);
        }
        
        public void SetActiveTextBubble5()
        {
            textBubble5.SetActive(true);
            textBubble3.SetActive(false);
            textBubble4.SetActive(false);
        }
    }
}