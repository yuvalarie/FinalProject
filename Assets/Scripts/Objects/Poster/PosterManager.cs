using System;
using UnityEngine;

namespace Objects.Poster
{
    public class PosterManager : MonoBehaviour
    {
        [SerializeField] private PosterData posterData;
        [SerializeField] private Transform posterContainer;

        private void Start()
        {
            if (posterData != null)
            {
                posterData.ClearData();
            }
        }

        public void SavePoster()
        {
            if (posterData == null)
            {
                Debug.LogError("PosterManager: PosterData reference is missing in the inspector!");
                return;
            }
            posterData.ClearData();
            var posterSprite = posterContainer.GetComponent<SpriteRenderer>();
            if (posterSprite != null && posterSprite.sprite != null)
            {
                posterData.originalPosterSize = new Vector2(
                    posterSprite.sprite.bounds.size.x, 
                    posterSprite.sprite.bounds.size.y
                );
            }
            foreach (Transform child in posterContainer)
            {
                var spriteRenderer = child.GetComponent<SpriteRenderer>();
                if (spriteRenderer == null) continue;
                int orderInLayer = spriteRenderer.sortingOrder;
                var entry = new StickerEntry
                {
                    localPos = child.localPosition,
                    localRot = child.localRotation,
                    localScale = child.localScale,
                    sortingOrder = orderInLayer,
                    sprite = spriteRenderer.sprite
                };
                posterData.placedStickers.Add(entry);
            }
        }
    }
}
