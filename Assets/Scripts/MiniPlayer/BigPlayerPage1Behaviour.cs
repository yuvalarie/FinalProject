using Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MiniPlayer
{
    public class BigPlayerPage1Behaviour : PlayerControllerBase
    {
        [SerializeField] private Sprite page1Sprite;
        [SerializeField] private PlayerControllerPage1 player;
        private SpriteRenderer _spriteRenderer;
        private bool _hasChangedSprite = false;
 
        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if (_hasChangedSprite) return;
            if (page1Sprite == null) return;
            _spriteRenderer.sprite = page1Sprite;
            _hasChangedSprite = true;
        }
        
        protected override void HandleMovement()
        {
            if (!_hasChangedSprite)
            {
                Rb.linearVelocity = Vector2.zero;
                return;
            }
            base.HandleMovement();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("End"))
            {
                gameObject.SetActive(false);
                player.canMove = true;
            }
        }
    }
}