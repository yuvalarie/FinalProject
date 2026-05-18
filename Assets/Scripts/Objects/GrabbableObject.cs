using UnityEngine;

namespace Objects
{
    public class GrabbableObject : MonoBehaviour
    {
        public enum ObjectState { Start, Held, Placed }

        [Header("Current State")]
        public ObjectState currentState = ObjectState.Start;
        
        [Header("Placement Settings")]
        [Tooltip("The specific trigger collider where this object belongs.")]
        public Collider2D targetDropSpot;
        public Collider2D[] validDropSpots;
        
        [Header("Visual Settings")]
        [SerializeField] private GameObject startSprite;
        [SerializeField] private GameObject heldSprite;
        [SerializeField] private GameObject placedSprite;

        private Vector3 _originalPosition;
        private Vector3 _startSpriteOriginalPosition;
        private Vector3 _heldSpriteOriginalPosition;
        private Vector3 _placedSpriteOriginalPosition;
        private int _startSpriteOriginalSortingOrder;
        private SpriteRenderer _startSpriteRenderer;
        
        private void Start()
        {
            _originalPosition = transform.position;
            if (startSprite != null) _startSpriteOriginalPosition = startSprite.transform.localPosition;
            if (heldSprite != null) _heldSpriteOriginalPosition = heldSprite.transform.localPosition;
            if (placedSprite != null) _placedSpriteOriginalPosition = placedSprite.transform.localPosition;
            if (startSprite != null)
            {
                _startSpriteRenderer = startSprite.GetComponent<SpriteRenderer>();
                if (_startSpriteRenderer != null) _startSpriteOriginalSortingOrder = _startSpriteRenderer.sortingOrder;
            }
            SwitchState();
        }

        public void SwitchState()
        {
            if (startSprite != null) startSprite.SetActive(false);
            if (heldSprite != null) heldSprite.SetActive(false);
            if (placedSprite != null) placedSprite.SetActive(false);

            switch (currentState)
            {
                case ObjectState.Start:
                    if (startSprite != null) startSprite.SetActive(true);
                    break;
                case ObjectState.Held:
                    if (heldSprite != null)
                    {
                        heldSprite.SetActive(true);
                        var spriteRenderer = heldSprite.GetComponent<SpriteRenderer>();
                        if (spriteRenderer != null) spriteRenderer.sortingOrder = 10;
                    }
                    break;
                case ObjectState.Placed:
                    if (placedSprite != null)
                    {
                        placedSprite.SetActive(true);
                        var spriteRenderer = placedSprite.GetComponent<SpriteRenderer>();
                        var targetSpriteRenderer = targetDropSpot.GetComponent<SpriteRenderer>();
                        if (spriteRenderer != null && targetSpriteRenderer != null) spriteRenderer.sortingOrder = targetSpriteRenderer.sortingOrder;
                    }
                    break;
            }
        }

        public void CenterChildren()
        {
            if (startSprite != null) startSprite.transform.localPosition = Vector3.zero;
            if (heldSprite != null) heldSprite.transform.localPosition = Vector3.zero;
            if (placedSprite != null) placedSprite.transform.localPosition = Vector3.zero;
        }
        
        public void ResetPosition()
        {
            transform.position = _originalPosition;
            _startSpriteRenderer.sortingOrder = _startSpriteOriginalSortingOrder;
            if (startSprite != null) startSprite.transform.localPosition = _startSpriteOriginalPosition;
            if (heldSprite != null) heldSprite.transform.localPosition = _heldSpriteOriginalPosition;
            if (placedSprite != null) placedSprite.transform.localPosition = _placedSpriteOriginalPosition;
        }
    }
}