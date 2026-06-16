using UnityEngine;

namespace Player
{
    public enum SizeSettings { Small, Medium, Large, ExtraLarge, None}
    
    [CreateAssetMenu(fileName = "NewPlayerArtSettings", menuName = "Player/Art Settings")]
    public class PlayerArtController : ScriptableObject
    {
        [Header("Small settings")] 
        public Sprite smallSprite;
        public float smallSize;
        public RuntimeAnimatorController smallAnimatorController;
        
        [Header("Medium + Large settings")]
        public Sprite mediumLargeSprite;
        public float mediumSize;
        public float largeSize;
        public RuntimeAnimatorController mediumLargeAnimatorController;
        
        [Header("Extra Large settings")]
        public Sprite extraLargeSprite;
        public float extraLargeSize;
        public RuntimeAnimatorController extraLargeAnimatorController;
    }
}