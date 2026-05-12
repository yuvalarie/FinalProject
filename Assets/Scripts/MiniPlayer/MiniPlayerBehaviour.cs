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
        
        // [Header("Transparency Link")]
        // [SerializeField, Tooltip("The big character's controller script.")]
        // private PlayerControllerBase bigPlayerController;
        //
        // [SerializeField, Tooltip("The big character's SpriteRenderer.")]
        // private SpriteRenderer bigSpriteRenderer;
        
        private SpriteRenderer mySpriteRenderer;

        private void Start()
        {
            mySpriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        //public bool IsTrans => bigPlayerController != null && bigPlayerController.IsTrans;

        private void Update()
        {
            // --- 1. Visual Link ---
            // if (mySpriteRenderer != null && bigSpriteRenderer != null)
            // {
            //     mySpriteRenderer.color = bigSpriteRenderer.color;
            // }
            
            // --- 2. Movement Link ---
            if (bigCharacter == null || littleFrameBoundary == null || 
                bigBottomLeftMarker == null || bigTopRightMarker == null) return;

            Bounds littleBounds = littleFrameBoundary.bounds;

            float normalizedX = Mathf.InverseLerp(bigBottomLeftMarker.position.x, bigTopRightMarker.position.x, bigCharacter.position.x);
            float normalizedY = Mathf.InverseLerp(bigBottomLeftMarker.position.y, bigTopRightMarker.position.y, bigCharacter.position.y);

            float targetX = Mathf.Lerp(littleBounds.min.x, littleBounds.max.x, normalizedX);
            float targetY = Mathf.Lerp(littleBounds.min.y, littleBounds.max.y, normalizedY);

            transform.position = new Vector3(targetX, targetY, transform.position.z);
        }
    }
}