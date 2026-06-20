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
        [SerializeField] private Sprite heldSprite;
        [SerializeField] private Category category;
        
        private SpriteRenderer _spriteRenderer;

        private bool isPickedUp;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void OnPickedUp()
        {
            if(!isPickedUp) Instantiate(gameObject);
            _spriteRenderer.sprite = heldSprite;
        }

        public void OnDropped()
        {
            Destroy(gameObject);
        }

        public Sprite GetSprite => _spriteRenderer.sprite;

        public Bounds StickerBounds => _spriteRenderer.bounds;

        public Category GetCategory => category;
        
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