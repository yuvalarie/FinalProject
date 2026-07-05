using System;
using Player;
using UnityEngine;

namespace MiniPlayer
{
    public class MiniPlayerBehaviour : MonoBehaviour
    {
        [SerializeField, Tooltip("The main character that this object mimics.")]
        private Transform bigCharacter;

        [SerializeField, Tooltip("The BoxCollider2D acting as the boundary for this character.")]
        private BoxCollider2D littleFrameBoundary;
    
        [Header("Big Character World Limits")]
        [SerializeField, Tooltip("Place an empty GameObject at the absolute bottom-left the big character can reach.")]
        private Transform bigBottomLeftMarker;

        [SerializeField, Tooltip("Place an empty GameObject at the absolute top-right the big character can reach.")]
        private Transform bigTopRightMarker;
        
        [Header("Mapping Adjustments")]
        [SerializeField, Tooltip("Check this to flip the horizontal movement (Right becomes Left).")]
        private bool invertX = true; 
        
        [SerializeField, Tooltip("Check this to flip the vertical movement (Up becomes Down).")]
        private bool invertY = false;
        
        [SerializeField] private float offsetX = 0f;
        [SerializeField] private float offsetY = 0f;
        
        private SpriteRenderer mySpriteRenderer;
        
        // ADD THIS: A variable to remember where we were last frame
        private float _previousX;

        private void Start()
        {
            mySpriteRenderer = GetComponent<SpriteRenderer>();
            // Initialize the previous position
            _previousX = transform.position.x;
        }

        private void Update()
        {
            if (bigCharacter == null || littleFrameBoundary == null || 
                bigBottomLeftMarker == null || bigTopRightMarker == null) return;

            Bounds littleBounds = littleFrameBoundary.bounds;

            float normalizedX = Mathf.InverseLerp(bigBottomLeftMarker.position.x, bigTopRightMarker.position.x, bigCharacter.position.x);
            float normalizedY = Mathf.InverseLerp(bigBottomLeftMarker.position.y, bigTopRightMarker.position.y, bigCharacter.position.y);
            
            if (invertX) normalizedX = 1f - normalizedX;
            if (invertY) normalizedY = 1f - normalizedY;
            
            float targetX = Mathf.Lerp(littleBounds.min.x, littleBounds.max.x, normalizedX) + offsetX;
            float targetY = Mathf.Lerp(littleBounds.min.y, littleBounds.max.y, normalizedY) + offsetY;

            // --- FLIP LOGIC FIX ---
            // Compare our new targetX to where we were last frame!
            if (targetX > _previousX)
            {
                mySpriteRenderer.flipX = false; // Moving right
            }
            else if (targetX < _previousX)
            {
                mySpriteRenderer.flipX = true; // Moving left
            }

            // Apply position
            transform.position = new Vector3(targetX, targetY, transform.position.z);
            
            // Save our current X so we can compare it again on the next frame
            _previousX = targetX;
        }
    }
}