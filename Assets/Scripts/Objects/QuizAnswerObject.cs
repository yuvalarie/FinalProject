using System;
using UnityEngine;

namespace Objects
{
    public class QuizAnswerObject : MonoBehaviour
    {
        [SerializeField] private Sprite originalSprite;
        [SerializeField] private Sprite hoverSprite;
        [SerializeField] private Sprite chosenSprite;
        [SerializeField] private int maxSortingOrder;
        [SerializeField] private int minSortingOrder;

        private SpriteRenderer spriteRenderer;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SwitchToHover()
        {
            spriteRenderer.sprite = hoverSprite;
            spriteRenderer.sortingOrder = maxSortingOrder;
        }
        
        public void SwitchToChosen()
        {
            spriteRenderer.sprite = chosenSprite;
        }
        
        public void SwitchToOriginal()
        {
            spriteRenderer.sprite = originalSprite;
            spriteRenderer.sortingOrder = minSortingOrder;
        }
    }
}