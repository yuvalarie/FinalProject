using System;
using UnityEngine;

namespace Objects.Poster
{
    public class PosterManager : MonoBehaviour
    {
        [SerializeField] private PosterData posterData;
        [SerializeField] private Transform posterContainer;
        // [SerializeField] private PosterObjectLibrary posterObjectLibrary;

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
