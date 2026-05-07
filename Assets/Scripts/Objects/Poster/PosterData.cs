using System.Collections.Generic;
using UnityEngine;

namespace Objects.Poster
{
    [CreateAssetMenu(fileName = "PosterData", menuName = "Scriptable Objects/PosterData")]
    public class PosterData : ScriptableObject
    {
        public List<StickerEntry> placedStickers = new List<StickerEntry>();
        public void ClearData()
        {
            placedStickers.Clear();
        }
    }
}
