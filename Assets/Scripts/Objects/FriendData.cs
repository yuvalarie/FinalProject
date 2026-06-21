using UnityEngine;

namespace Objects
{
    [CreateAssetMenu(fileName = "FriendData", menuName = "MiniGame2/Friend Data")]
    public class FriendData : ScriptableObject
    {
        [Header("Identity")]
        public string friendName;
        public int friendId;
        
        [Header("Sprites")]
        public Sprite appProfileSprite; // The sprite shown in the app.
        public Sprite roamingSprite; // The sprite shown when the friend is roaming in the scene.
        public Sprite thrownSprite; // The sprite shown when the friend is thrown into the portal. If there is no sprite, will use the roaming sprite as a fallback.
        public Sprite speechBubbleSprite; // The sprite shown in the speech bubble when the friend is talking. Might change later to text if we change the design, but for now we can just use a sprite to keep it simple.
        
        [Header("Area")]
        public MiniGame2FrameArea assignedArea; // The area that this friend is assigned to. This is used to determine where the friend will roam.
    
    }
}
