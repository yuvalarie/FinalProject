using System.Collections.Generic;
using UnityEngine;

namespace Objects.Poster
{
    [CreateAssetMenu(fileName = "PosterData", menuName = "Scriptable Objects/PosterData")]
    public class PosterData : ScriptableObject
    {
        [Tooltip("Records the physical size (X and Y) of the poster when saved.")]
        public Vector2 originalPosterSize = Vector2.one;
        
        public List<StickerEntry> placedStickers = new List<StickerEntry>();
        public void ClearData()
        {
            placedStickers.Clear();
        }
    }
}
