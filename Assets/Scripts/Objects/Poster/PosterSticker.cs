using UnityEngine;

namespace Objects.Poster
{
    public class PosterSticker : MonoBehaviour
    {
        [Tooltip("The ID MUST match the ID of the stickerID in the PosterObjectLibrary scriptable object.")]
        [SerializeField] private int stickerID;
        public int StickerID => stickerID;
    }
}
