using System;
using UnityEngine;

namespace Objects.Poster
{
    public class PosterSticker : MonoBehaviour
    {
        [Tooltip("The ID MUST match the ID of the stickerID in the PosterObjectLibrary scriptable object.")]
        [SerializeField] private int stickerID;
        public int StickerID => stickerID;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer == null)
            {
                Debug.LogError($"PosterSticker on {gameObject.name} is missing a SpriteRenderer component.");
            }
        }
        
        public void DroppedOnPoster(int orderInLayer)
        {
            // This method can be called when the sticker is dropped onto the poster.
            // You can add any additional logic here if needed, such as snapping to a grid or playing a sound effect.
            Debug.Log($"Sticker with ID {stickerID} dropped on poster at position {transform.localPosition}.");
            _spriteRenderer.sortingOrder = orderInLayer; // Set the sorting order based on the order in which it was dropped. You can modify this logic as needed.
        }
    }
}
