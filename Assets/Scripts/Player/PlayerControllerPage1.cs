using System;
using Transitions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerControllerPage1 : PlayerControllerBase
    {
        [SerializeField] private Sprite page1Sprite;
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
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Transition"))
            {
                var rowTransition = other.GetComponent<RowTransition>();
                if (rowTransition == null) return;
                transform.position = rowTransition.destinationSpawn.position;
                transform.localScale = new Vector3(rowTransition.targetScale, rowTransition.targetScale, 1f);
                _spriteRenderer.sortingOrder = rowTransition.sortingOrder;
            }

            if (other.CompareTag("End"))
            {
                gameObject.SetActive(false);
            }
        }
    }
}