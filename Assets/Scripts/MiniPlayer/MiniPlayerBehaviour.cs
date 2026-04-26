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

        private void Update()
        {
            if (bigCharacter == null || littleFrameBoundary == null || 
                bigBottomLeftMarker == null || bigTopRightMarker == null) return;

            Bounds littleBounds = littleFrameBoundary.bounds;

            // 1. Calculate the Big Guy's relative position (0.0 to 1.0) using the two corner markers
            float normalizedX = Mathf.InverseLerp(bigBottomLeftMarker.position.x, bigTopRightMarker.position.x, bigCharacter.position.x);
            float normalizedY = Mathf.InverseLerp(bigBottomLeftMarker.position.y, bigTopRightMarker.position.y, bigCharacter.position.y);

            // 2. Apply that exact same percentage to the Little Guy's frame bounds
            float targetX = Mathf.Lerp(littleBounds.min.x, littleBounds.max.x, normalizedX);
            float targetY = Mathf.Lerp(littleBounds.min.y, littleBounds.max.y, normalizedY);

            // 3. Move the little guy to this calculated position instantly
            transform.position = new Vector3(targetX, targetY, transform.position.z);
        }
    }
}