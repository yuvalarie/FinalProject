using UnityEngine;

namespace Objects.Poster
{
    public class PosterManager : MonoBehaviour
    {
        [SerializeField] private PosterData posterData;
        [SerializeField] private Transform posterContainer;
        // [SerializeField] private PosterObjectLibrary posterObjectLibrary;

        public void SavePoster()
        {
            posterData.ClearData();
            foreach (Transform child in posterContainer)
            {
                var sticker = child.GetComponent<PosterSticker>();
                if (sticker == null) continue;
                var entry = new StickerEntry
                {
                    id = sticker.StickerID,
                    localPos = child.localPosition,
                    localRot = child.localRotation,
                    localScale = child.localScale
                };
                posterData.placedStickers.Add(entry);
            }
        }
    }
}
