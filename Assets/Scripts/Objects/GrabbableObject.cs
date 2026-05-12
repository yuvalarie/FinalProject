using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;

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
        
        [Header("Visual Settings")]
        [SerializeField] Sprite heldSprite;
        [SerializeField] Sprite placedSprite;
        
        private SpriteRenderer _spriteRenderer;
        
        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SwitchState()
        {
            switch (currentState)
            {
                case ObjectState.Held:
                    if (heldSprite != null) _spriteRenderer.sprite = heldSprite;
                    break;
                case ObjectState.Placed:
                    if (placedSprite != null) _spriteRenderer.sprite = placedSprite;
                    break;
                case ObjectState.Start:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}