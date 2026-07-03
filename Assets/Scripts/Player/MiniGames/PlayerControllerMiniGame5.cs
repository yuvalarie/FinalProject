using System;
using System.Collections;
using Managers;
using Objects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.MiniGames
{
    public class PlayerControllerMiniGame5 : PlayerControllerBase
    {
        [Header("Catching Settings")]
        [SerializeField, Tooltip("Drag the CatchZone child object's BoxCollider2D here.")] 
        private Collider2D catchZoneCollider;

        [Header("Game Settings")]
        [SerializeField] private GameObject openingText;
        [SerializeField] private GameObject trashCan;
        [SerializeField] private Transform trashCanPosition;
        [SerializeField] private float textDuration;
        [SerializeField] private NoteSpawner noteSpawner;
        [SerializeField] private Sprite secondSprite;
        [SerializeField] private Animator secondAnimator;
        [SerializeField] private float size;
        [SerializeField] private Transform holdSlot;

        private bool _atTrashCan;
        private bool _pickedUpTrashCan;
        
        protected override void Start()
        {
            base.Start();
            secondAnimator.enabled = false;
            SceneLoader.Instance?.PreloadScene(nextSceneName);
        }

        private void Update()
        {
            if (_pickedUpTrashCan)
            {
                AutomaticCatchCheck();
            }
        }

        private void AutomaticCatchCheck()
        {
            if (catchZoneCollider == null) return;

            Collider2D[] overlappedObjects = Physics2D.OverlapBoxAll(
                catchZoneCollider.bounds.center, 
                catchZoneCollider.bounds.size, 
                0f
            );

            foreach (Collider2D col in overlappedObjects)
            {
                FallingNote note = col.GetComponent<FallingNote>();
                
                if (note != null)
                {
                    note.CatchByPlayer(holdSlot);
                }
            }
        }

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            if (_atTrashCan && !_pickedUpTrashCan)
            {
                _pickedUpTrashCan = true;
                trashCan.SetActive(false);
                SpriteRenderer.sprite = secondSprite;
                Animator.enabled = false;
                secondAnimator.enabled = true;
                transform.localScale = new Vector3(size, size, 1f);
                StartCoroutine(StartGameCoroutine());
            }
        }

        private IEnumerator StartGameCoroutine()
        {
            openingText.SetActive(true);
            yield return new WaitForSeconds(textDuration);
            openingText.SetActive(false);
            noteSpawner.StartGame();
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Trash") && !_pickedUpTrashCan) _atTrashCan = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Trash")) _atTrashCan = false;
        }

        public IEnumerator GameEnded()
        {
            yield return new WaitForSeconds(3f);
            SceneLoader.Instance?.ActivatePreloadedScene();
        }
    }
}