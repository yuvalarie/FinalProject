using System.Collections;
using Managers;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Player
{
    public class PlayerControllerPage8 : PlayerControllerBase
    {
        [SerializeField] private GameObject frame2;
        [SerializeField] private GameObject frame3;
        [SerializeField] private GameObject frame4;
        [SerializeField] private GameObject frame4Part2;
        [SerializeField] private GameObject textBubble1;
        [SerializeField] private GameObject textBubble2;
        [SerializeField] private GameObject letter;
        [SerializeField] private Collider2D frame5Collider;
        [SerializeField] private Sprite frame5Sprite;
        [SerializeField] private Vector3 frame5Size;
        [SerializeField] private Collider2D frame1Collider;
        [SerializeField] private Sprite frame1Sprite;
        [SerializeField] private Vector3 frame1Size;

        private bool canMove = false;
        private bool textBubble1Shown = false;
        private bool textBubble2Shown = false;
        private bool hasLetterShown = false;
        private bool hasLetterClosed = false;
        private SpriteRenderer _spriteRenderer;

        protected override void Start()
        {
            base.Start();
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            StartCoroutine(SceneSequence());
        }

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (!textBubble1Shown) return;
            if (textBubble1Shown && !textBubble2Shown && !hasLetterShown)
            {
                textBubble1.SetActive(false);
                textBubble2.SetActive(true);
                textBubble2Shown = true;
            }

            else if (textBubble1Shown && textBubble2Shown && !hasLetterShown)
            {
                textBubble2.SetActive(false);
                letter.SetActive(true);
                hasLetterShown = true;
            }
            
            else if (textBubble1Shown && textBubble2Shown && hasLetterShown)
            {
                letter.SetActive(false);
                hasLetterClosed = true;
            }
        }

        protected override void HandleMovement()
        {
            if (canMove) base.HandleMovement();
        }

        private IEnumerator SceneSequence()
        {
            yield return new WaitForSeconds(5f);
            frame2.SetActive(true);
            yield return new WaitForSeconds(1f);
            frame3.SetActive(true);
            yield return new WaitForSeconds(1f);
            frame4.SetActive(true);
            yield return new WaitForSeconds(1f);
            frame4Part2.SetActive(true);
            canMove = true;
            yield return new WaitForSeconds(1f);
            textBubble1.SetActive(true);
            textBubble1Shown = true;
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("End") && hasLetterClosed)
            {
                SceneLoader.Instance?.ActivatePreloadedScene();
            }
            if (other == frame5Collider)
            {
                _spriteRenderer.sprite = frame5Sprite;
                transform.localScale = frame5Size;
            }
            if (other == frame1Collider)
            {
                _spriteRenderer.sprite = frame1Sprite;
                transform.localScale = frame1Size;
            }
        }
    }
}