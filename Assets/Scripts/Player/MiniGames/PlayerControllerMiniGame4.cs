using System;
using System.Collections;
using Managers;
using Objects.Poster;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerControllerMiniGame4 : PlayerControllerBase 
    {
        [SerializeField, Tooltip("Transform where the held object will sit.")]
        private Transform holdSlot;

        [SerializeField, Tooltip("The layer used for grabbable objects.")]
        private LayerMask grabbableLayer;
        
        [SerializeField, Tooltip("How far the player can reach to grab an object.")]
        private float grabRadius = 1f;
        
        [SerializeField, Tooltip("The layer used for valid drop zones.")]
        private LayerMask dropZoneLayer;
        
        [SerializeField] private int startingStickerOrderInLayer = 0;

        [Header("Print Settings")]
        [SerializeField] private Collider2D printCollider;
        [SerializeField] private Animator paperAnimator;
        [SerializeField] private float animationDuration;
        [SerializeField] private SpriteRenderer printSpriteRenderer;
        [SerializeField] private Sprite printOnSprite;
        [SerializeField] private PosterManager posterManager;
        
        private GameObject _heldItem;
        private StickerObject _heldSticker;
        private int _stickerOrder;
        private int _playerSortingOrder;
        private bool _atPrintZone;

        private bool _category1;
        private bool _category2;
        private bool _category3;
        private bool _category4;

        protected override void Start()
        {
            base.Start();
            _stickerOrder = startingStickerOrderInLayer;
            _playerSortingOrder = SpriteRenderer.sortingOrder;
        }

        private void Update()
        {
            if(_category1 && _category2 && _category3 && _category4) printSpriteRenderer.sprite = printOnSprite;
        }

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if(!context.performed) return;
            if (_atPrintZone)
            {
                if (!(_category1 && _category2 && _category3 && _category4)) return;
                StartCoroutine(PrintCoroutine());
            }
            if (_heldItem == null) TryPickUp();
            else DropItem();
        }

        private IEnumerator PrintCoroutine()
        {
            posterManager.SavePoster();
            paperAnimator.SetTrigger("Print");
            yield return new WaitForSeconds(animationDuration);
            SceneLoader.Instance.ActivatePreloadedScene();
        }

        private void SetCategory(Category category)
        {
            switch (category)
            {
                case Category.Category1:
                    _category1 = true;
                    break;
                case Category.Category2:
                    _category2 = true;
                    break;
                case Category.Category3:
                    _category3 = true;
                    break;
                case Category.Category4:
                    _category4 = true;
                    break;
            }
        }

        private void TryPickUp()
        {
            Debug.Log("Attempting to pick up item...");

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, grabRadius, grabbableLayer);

            if (hits.Length > 0)
            {
                Collider2D closestCollider = null;
                float closestDistance = Mathf.Infinity;

                foreach (Collider2D hit in hits)
                {
                    float distance = Vector2.Distance(transform.position, hit.transform.position);
                    
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestCollider = hit;
                    }
                }

                if (closestCollider == null) return;
                
                _heldSticker = closestCollider.GetComponent<StickerObject>();
                if (_heldSticker == null)
                {
                    Debug.LogWarning($"Wait, '{closestCollider.name}' isn't a PosterSticker! Are you sure it's on the right layer?");
                    return;
                }
                
                _heldSticker.OnPickedUp();
                
                _heldItem = _heldSticker.gameObject;
                
                _heldItem.transform.position = holdSlot.position;
                _heldItem.transform.SetParent(holdSlot);
                _heldSticker.SetSortingOrder(_playerSortingOrder + 1);
            }
            else
            {
                Debug.Log("FAILED: Nothing found on the Grabbable layer within the grab radius.");
            }
        }

        private void DropItem()
        {
            Debug.Log("Attempting to drop item...");
            Collider2D dropZone = Physics2D.OverlapCircle(transform.position, grabRadius, dropZoneLayer);

            if (dropZone != null)
            {
                var zone = dropZone.gameObject;
                if (zone.CompareTag($"Original"))
                {
                    _heldSticker.OnDropped();
                    return;
                }
                
                Bounds paperBounds = dropZone.bounds;
                Bounds stickerBounds = _heldSticker.StickerBounds;
                
                bool isFullyInside = 
                    stickerBounds.min.x >= paperBounds.min.x &&
                    stickerBounds.max.x <= paperBounds.max.x &&
                    stickerBounds.min.y >= paperBounds.min.y &&
                    stickerBounds.max.y <= paperBounds.max.y;
                
                if (!isFullyInside)
                {
                    Debug.Log("FAILED: The sticker is sticking out of the paper! Move it inward.");
                    return;
                }

                _heldItem.transform.SetParent(dropZone.transform);
                
                _heldSticker.SetPickedUp();
                _heldSticker.SetSortingOrder(_stickerOrder);
                _stickerOrder++;
                
                SetCategory(_heldSticker.GetCategory);
                
                _heldItem = null;
                _heldSticker = null;
            }
            else
            {
                Debug.Log("FAILED: Cannot drop here. You must be in a drop zone!");
            }
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other == printCollider) _atPrintZone = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other == printCollider) _atPrintZone = false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, grabRadius);
        }
    }
}
