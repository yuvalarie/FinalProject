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
        public Vector2 smallOffset = new Vector2(0,0);
        
        [Header("Medium + Large settings")]
        public Sprite mediumLargeSprite;
        public float mediumSize;
        public float largeSize;
        public RuntimeAnimatorController mediumLargeAnimatorController;
        public Vector2 mediumLargeOffset = new Vector2(7.5f, 0.5f);
        
        [Header("Extra Large settings")]
        public Sprite extraLargeSprite;
        public float extraLargeSize;
        public RuntimeAnimatorController extraLargeAnimatorController;
        public Vector2 extraLargeOffset = new Vector2(0,0);
    }
}