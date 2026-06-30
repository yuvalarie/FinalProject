using System;
using DG.Tweening;
using UnityEngine;

namespace Objects
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class QuizAnswerObject : MonoBehaviour
    {
        [SerializeField] private Sprite originalSprite;
        [SerializeField] private Sprite hoverSprite;
        [SerializeField] private Sprite chosenSprite;
        [SerializeField] private int maxSortingOrder;
        [SerializeField] private int minSortingOrder;
        [SerializeField] private float activationScaleDuration = 0.2f;
        [SerializeField] private Ease activationScaleEase = Ease.OutBack;

        private SpriteRenderer spriteRenderer;
        private Vector3 activeScale;
        private Tween activationTween;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                enabled = false;
                return;
            }

            activeScale = transform.localScale;
        }

        private void OnEnable()
        {
            PlayActivationAnimation();
        }

        private void OnDisable()
        {
            activationTween?.Kill();
            transform.localScale = activeScale;
        }

        private void PlayActivationAnimation()
        {
            activationTween?.Kill();
            transform.localScale = Vector3.zero;
            activationTween = transform.DOScale(activeScale, activationScaleDuration).SetEase(activationScaleEase);
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