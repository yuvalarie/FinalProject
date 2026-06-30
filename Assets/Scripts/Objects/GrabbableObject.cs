using Audio.AudioEmitters;
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
        
        [Header("Audio Settings")]
        [SerializeField] private AudioEmitterBase pickupAudioEmitter;
        [SerializeField] private AudioEmitterBase placedAudioEmitter;
        
        [Tooltip("If true, interacting with this instantly teleports it to its drop spot.")]
        public bool isInstantPlacement = false;

        private Transform _originalParent;
        
        private Vector3 _originalLocalPosition;
        private Quaternion _originalLocalRotation;
        private Vector3 _originalLocalScale;
        
        private Vector3 _startSpriteOriginalPosition;
        private Vector3 _heldSpriteOriginalPosition;
        private Vector3 _placedSpriteOriginalPosition;
        private int _startSpriteOriginalSortingOrder;
        private SpriteRenderer _startSpriteRenderer;
        
        private void Start()
        {
            _originalParent = transform.parent;
            
            _originalLocalPosition = transform.localPosition;
            _originalLocalRotation = transform.localRotation;
            _originalLocalScale = transform.localScale;
            
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
                        if (spriteRenderer != null) spriteRenderer.sortingOrder = 15;
                        pickupAudioEmitter?.PlayAudioOnce();
                    }
                    break;
                case ObjectState.Placed:
                    if (placedSprite != null)
                    {
                        placedSprite.SetActive(true);
                        SpriteRenderer targetSpriteRenderer = null;
                        if (targetDropSpot != null)
                        {
                            targetSpriteRenderer = targetDropSpot.GetComponent<SpriteRenderer>();
                        }
                        else
                        {
                            if (validDropSpots.Length > 0) targetSpriteRenderer = validDropSpots[0].GetComponent<SpriteRenderer>();
                        }
                        var spriteRenderer = placedSprite.GetComponent<SpriteRenderer>();
                        if (spriteRenderer != null && targetSpriteRenderer != null) spriteRenderer.sortingOrder = targetSpriteRenderer.sortingOrder + 1;
                        if (isInstantPlacement) spriteRenderer.sortingOrder = 3;
                        placedAudioEmitter?.PlayAudioOnce();
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
            transform.SetParent(_originalParent);
            transform.localPosition = _originalLocalPosition;
            transform.localRotation = _originalLocalRotation;
            transform.localScale = _originalLocalScale;
            
            if (_startSpriteRenderer != null)
            {
                _startSpriteRenderer.sortingOrder = _startSpriteOriginalSortingOrder;
            }
            if (startSprite != null) startSprite.transform.localPosition = _startSpriteOriginalPosition;
            if (heldSprite != null) heldSprite.transform.localPosition = _heldSpriteOriginalPosition;
            if (placedSprite != null) placedSprite.transform.localPosition = _placedSpriteOriginalPosition;
        }
    }
}