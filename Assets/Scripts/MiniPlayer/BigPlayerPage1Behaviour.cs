using System.Collections; // Required for Coroutines
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MiniPlayer
{
    public class BigPlayerPage1Behaviour : PlayerControllerBase
    {
        private static readonly int Answer = Animator.StringToHash("Answer");
        [SerializeField] private Sprite page1Sprite;
        [SerializeField] private PlayerControllerPage1 player;
        [SerializeField] private Animator phoneAnimator;
        [SerializeField] private Vector3 size;
        [SerializeField] private Vector3 position;
        [SerializeField] private RuntimeAnimatorController miniPlayerAnimator;
        [SerializeField] private RuntimeAnimatorController playerAnimator;
        
        [Tooltip("Set this to the exact length of the Answer animation in seconds.")]
        [SerializeField] private float animationDuration = 1.5f;
        
        private bool _hasChangedSprite = false;
        private bool _canMove = false;

        protected override void Start()
        {
            base.Start();
            SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            Animator = GetComponentInChildren<Animator>();
            Animator.runtimeAnimatorController = miniPlayerAnimator;
        }

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if (_hasChangedSprite) return;
            if (page1Sprite == null) return;

            SpriteRenderer.sprite = page1Sprite;
            SpriteRenderer.sortingOrder -= 2;
            transform.localScale = size;
            transform.localPosition = position;
            
            phoneAnimator.SetTrigger(Answer);
            _hasChangedSprite = true;
            Animator.runtimeAnimatorController = playerAnimator;
            
            StartCoroutine(WaitForAnimationRoutine());
        }
        
        private IEnumerator WaitForAnimationRoutine()
        {
            yield return new WaitForSeconds(animationDuration); 
            _canMove = true; 
        }
        
        protected override void HandleMovement()
        {
            if (!_canMove) 
            {
                Rb.linearVelocity = Vector2.zero;
                return;
            }
            base.HandleMovement();
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("End"))
            {
                gameObject.SetActive(false);
                player.EnableMovement();
                other.gameObject.SetActive(false);
            }
        }
    }
}