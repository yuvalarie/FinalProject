using UnityEngine;
using UnityEngine.Serialization;

namespace Objects.Poster
{
    public class PosterDisplay : MonoBehaviour
    {
        [Tooltip("The PosterData Scriptable Object that holds the data for the stickers placed on the poster. This should be assigned in the inspector.")]
        [SerializeField] private PosterData posterData;
        [SerializeField] private PosterData defaultPoster;
        [SerializeField] private int sortingOrder;
        
        void Start()
        {
            //LoadPoster();
        }

        public void LoadPoster()
        {
            // Clear existing stickers
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            
            Vector2 currentPosterSize = Vector2.one;
            var mySprite = GetComponent<SpriteRenderer>();
            if (mySprite != null && mySprite.sprite != null)
            {
                currentPosterSize = new Vector2(
                    mySprite.sprite.bounds.size.x, 
                    mySprite.sprite.bounds.size.y
                );
            }
            
            Vector2 scaleRatio = Vector2.one;
            if (posterData.originalPosterSize.x > 0 && posterData.originalPosterSize.y > 0)
            {
                scaleRatio = new Vector2(
                    currentPosterSize.x / posterData.originalPosterSize.x,
                    currentPosterSize.y / posterData.originalPosterSize.y
                );
            }

            var relevantPoster = posterData.placedStickers.Count == 0 ? defaultPoster : posterData;

            // Recreate stickers from saved data
            foreach (var entry in relevantPoster.placedStickers)
            {
                if (entry.sprite == null)
                {
                    Debug.LogWarning("StickerEntry contains a null sprite. Skipping this sticker.");
                    continue;
                }

                var stickerObj = new GameObject("Sticker");
                stickerObj.transform.SetParent(transform, false);
                stickerObj.transform.localPosition = new Vector3(
                    entry.localPos.x * scaleRatio.x,
                    entry.localPos.y * scaleRatio.y,
                    entry.localPos.z
                );
        
                stickerObj.transform.localRotation = entry.localRot; 
        
                stickerObj.transform.localScale = new Vector3(
                    entry.localScale.x * scaleRatio.x,
                    entry.localScale.y * scaleRatio.y,
                    entry.localScale.z
                );

                var spriteRenderer = stickerObj.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = entry.sprite;
                spriteRenderer.sortingOrder = entry.sortingOrder + sortingOrder;
            }
        }
    }
}
