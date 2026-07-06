using System;
using UnityEngine;

namespace Objects.Poster
{
    public enum Category
    {
        Category1,
        Category2,
        Category3,
        Category4
    };
    public class StickerObject : MonoBehaviour
    {
        [SerializeField] private Category category;

        [Header("Visual States")] 
        [SerializeField] private Sprite startSprite;
        [SerializeField] private Sprite hoveredSprite;
        [SerializeField] private Sprite heldSprite;
        [SerializeField] private Sprite placedSprite;
        [SerializeField] private int maxSortingOrder;
        [SerializeField] private int ogSortingOrder;
        
        private SpriteRenderer _spriteRenderer;
        private bool _isHovered;

        private bool isPickedUp;
        private bool isPlaced;
        
        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = startSprite;
            _spriteRenderer.sortingOrder = ogSortingOrder;
        }

        public void SetHovered(bool isHovered)
        {
            if (isPlaced) return;
            _spriteRenderer.sprite = isHovered ? hoveredSprite : startSprite;
            _spriteRenderer.sortingOrder = isHovered ? maxSortingOrder : ogSortingOrder;
            _isHovered = isHovered;
        }
        
        public void OnPickedUp()
        {
            if(!isPickedUp) Instantiate(gameObject);
            _spriteRenderer.sprite = heldSprite;
            Debug.Log($"sorting order {_spriteRenderer.sortingOrder}");
        }

        public void OnDropped()
        {
            Destroy(gameObject);
        }

        public void OnPlaced()
        {
            _spriteRenderer.sprite = placedSprite;
            isPlaced = true;
        }

        public Sprite GetSprite => _spriteRenderer.sprite;

        public Bounds StickerBounds => _spriteRenderer.bounds;

        public Category GetCategory => category;

        public bool IsPlaced => isPlaced;
        
        public void SetPickedUp()
        {
            isPickedUp = true;
        }

        public void SetSortingOrder(int order)
        {
            _spriteRenderer.sortingOrder = order;
        }
    }
}