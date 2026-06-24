using UnityEngine;
using UnityEngine.Serialization;

namespace Objects.Poster
{
    public class PosterDisplay : MonoBehaviour
    {
        [Tooltip("The PosterData Scriptable Object that holds the data for the stickers placed on the poster. This should be assigned in the inspector.")]
        [SerializeField] private PosterData posterData;
        
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

            // Recreate stickers from saved data
            foreach (var entry in posterData.placedStickers)
            {
                if (entry.sprite == null)
                {
                    Debug.LogWarning("StickerEntry contains a null sprite. Skipping this sticker.");
                    continue;
                }

                var stickerObj = new GameObject("Sticker");
                stickerObj.transform.SetParent(transform, false);
                stickerObj.transform.localPosition = entry.localPos;
                stickerObj.transform.localRotation = entry.localRot;
                stickerObj.transform.localScale = entry.localScale;

                var spriteRenderer = stickerObj.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = entry.sprite;
                spriteRenderer.sortingOrder = entry.sortingOrder;
            }
        }
    }
}
