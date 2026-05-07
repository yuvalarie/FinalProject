using System;
using System.Collections.Generic;
using UnityEngine;

namespace Objects.Poster
{
    [Serializable]
    public class PosterLibraryEntry
    {
        
        public int stickerID; // The ID MUST match the stickerID in the PosterSticker script.
        public string displayName; // for debugging and organization purposes, not used in game.
        public GameObject stickerPrefab;
        
    }
    
    [CreateAssetMenu(fileName = "PosterObjectLibrary", menuName = "Scriptable Objects/PosterObjectLibrary")]
    public class PosterObjectLibrary : ScriptableObject
    {
        public List<PosterLibraryEntry> posterEntries = new List<PosterLibraryEntry>();
        
        public GameObject GetStickerPrefabByID(int id)
        {
            var entry = posterEntries.Find(e => e.stickerID == id);
            if (entry != null) return entry.stickerPrefab;
            Debug.LogError($"Sticker ID {id} not found in PosterObjectLibrary.");
            return null;
        }
    }
}
