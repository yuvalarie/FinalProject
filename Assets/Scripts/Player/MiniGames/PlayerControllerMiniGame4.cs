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
        private StickerObject _currentlyHoveredSticker;
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
            SceneLoader.Instance?.PreloadScene(nextSceneName);
            _stickerOrder = startingStickerOrderInLayer;
            SpriteRenderer = GetComponent<SpriteRenderer>();
            _playerSortingOrder = SpriteRenderer.sortingOrder;
        }

        private void HandleHoverLogic()
        {
            if (_heldItem != null || _atPrintZone)
            {
                if (_currentlyHoveredSticker != null)
                {
                    _currentlyHoveredSticker.SetHovered(false);
                    _currentlyHoveredSticker = null;
                }
                return;
            }

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, grabRadius, grabbableLayer);
            StickerObject closestSticker = null;
            float closestDistance = Mathf.Infinity;

            foreach (Collider2D hit in hits)
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestSticker = hit.GetComponent<StickerObject>();
                }
            }

            // If our closest sticker has changed, update the visual states
            if (closestSticker != _currentlyHoveredSticker)
            {
                if (_currentlyHoveredSticker != null)
                {
                    _currentlyHoveredSticker.SetHovered(false); 
                }
                
                _currentlyHoveredSticker = closestSticker;
                
                if (_currentlyHoveredSticker != null)
                {
                    _currentlyHoveredSticker.SetHovered(true); 
                }
            }
        }

        private void Update()
        {
            if(_category1 && _category2 && _category3 && _category4) printSpriteRenderer.sprite = printOnSprite;
            HandleHoverLogic();
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
            if (_currentlyHoveredSticker == null)
            {
                Debug.Log("FAILED: Nothing hovered to pick up.");
                return;
            }

            Debug.Log("Attempting to pick up item...");

            _heldSticker = _currentlyHoveredSticker;
            _heldSticker.OnPickedUp();
            
            _heldItem = _heldSticker.gameObject;
            _heldItem.transform.position = holdSlot.position;
            _heldItem.transform.SetParent(holdSlot);
            _heldItem.transform.localPosition = new Vector3(0f, 0f, -0.01f);
            _heldSticker.SetSortingOrder(_playerSortingOrder + 1);
            
            _currentlyHoveredSticker = null;
        }

        private void DropItem()
        {
            Debug.Log("Attempting to drop item...");
            
            // Get ALL drop zones within grab radius
            Collider2D[] dropZones = Physics2D.OverlapCircleAll(transform.position, grabRadius, dropZoneLayer);
            
            Collider2D closestZone = null;
            float closestDistance = Mathf.Infinity;

            // Find the absolute closest drop zone to the player
            foreach (Collider2D zone in dropZones)
            {
                float distance = Vector2.Distance(transform.position, zone.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestZone = zone;
                }
            }

            // If we found a valid drop zone nearby...
            if (closestZone != null)
            {
                // 1. Did we drop it back on the original table?
                if (closestZone.CompareTag("Original"))
                {
                    Debug.Log("Dropped back to the original table.");
                    _heldSticker.OnDropped(); // Destroys the clone in hand
                    
                    // Clear the hand references
                    _heldItem = null;
                    _heldSticker = null;
                    return;
                }
                
                // 2. Otherwise, we are dropping on the paper. Do the bounds check!
                Bounds paperBounds = closestZone.bounds;
                Bounds stickerBounds = _heldSticker.StickerBounds;
                
                bool isFullyInside = 
                    stickerBounds.min.x >= paperBounds.min.x &&
                    stickerBounds.max.x <= paperBounds.max.x &&
                    stickerBounds.min.y >= paperBounds.min.y &&
                    stickerBounds.max.y <= paperBounds.max.y;
                
                if (!isFullyInside)
                {
                    Debug.Log("FAILED: The sticker is sticking out of the paper! Staying in hand.");
                    return; // By returning early, the item stays in the player's hand!
                }

                // 3. Successfully placed on the paper
                _heldItem.transform.SetParent(closestZone.transform);
                
                _heldSticker.SetPickedUp();
                _heldSticker.OnPlaced();
                _heldSticker.SetSortingOrder(_stickerOrder);
                _stickerOrder++;
                
                SetCategory(_heldSticker.GetCategory);
                
                // Clear the hand references
                _heldItem = null;
                _heldSticker = null;
            }
            else
            {
                Debug.Log("FAILED: Cannot drop here. You must be in a drop zone! Staying in hand.");
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
