using UnityEngine;

namespace Objects.Poster
{
    public class PosterDisplay : MonoBehaviour
    {
        [Tooltip("The PosterData Scriptable Object that holds the data for the stickers placed on the poster. This should be assigned in the inspector.")]
        [SerializeField] private PosterData posterDataSO;
        [SerializeField] private PosterObjectLibrary posterObjectLibrary;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            LoadPoster();
        }

        public void LoadPoster()
        {
            // Clear existing stickers
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            // Instantiate stickers based on the data in posterDataSO
            foreach (var entry in posterDataSO.placedStickers)
            {
                var stickerPrefab = posterObjectLibrary.GetStickerPrefabByID(entry.id);
                if (stickerPrefab != null)
                {
                    var stickerInstance = Instantiate(stickerPrefab, transform);
                    stickerInstance.transform.localPosition = entry.localPos;
                    stickerInstance.transform.localRotation = entry.localRot;
                    stickerInstance.transform.localScale = entry.localScale;
                    stickerInstance.GetComponent<SpriteRenderer>().sortingOrder = entry.sortingOrder;

                    // remove the unnecessary components from the instantiated sticker prefab
                    if (stickerInstance.TryGetComponent<Rigidbody2D>(out var rb)) rb.simulated = false;
                    if (stickerInstance.TryGetComponent<Collider2D>(out var col)) col.enabled = false;
                    if (stickerInstance.TryGetComponent<PosterSticker>(out var script)) Destroy(script);
                }
                else
                {
                    Debug.LogWarning($"Sticker with ID {entry.id} not found in PosterObjectLibrary.");
                }
            }
        }
    }
}
