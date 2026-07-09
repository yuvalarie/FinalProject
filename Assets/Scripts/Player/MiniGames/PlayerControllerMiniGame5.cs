using System;
using System.Collections;
using System.Collections.Generic;
using Audio.AudioEmitters;
using Managers;
using NUnit.Framework;
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

        [Header("Audio")]
        [SerializeField] private AudioEmitterBase ballotGrabbedEmitter;
        [SerializeField] private AudioEmitterBase ashtrayPickupEmitter;

        [Header("End Sequence Settings")]
        [SerializeField] private GameObject endText;
        [SerializeField] private float endTextDuration;
        [SerializeField, Tooltip("The Animator to play at the end of the game.")]
        private GameObject endAnimator;
        [SerializeField, Tooltip("The sprite showing all the caught notes.")]
        private Sprite allNotesCaughtSprite;
        [SerializeField] private float notesCaughtScale;
        [SerializeField, Tooltip("How long the animation plays before changing to the final sprite.")]
        private float endAnimationDuration = 1.5f;
        [SerializeField, Tooltip("How long to show the caught notes before loading the next scene.")]
        private float timeBeforeSceneLoad = 1.5f;

        private bool _atTrashCan;
        private bool _pickedUpTrashCan;
        private List<FallingNote> _caughtNotes = new List<FallingNote>();
        
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
                    if (note.CatchByPlayer(holdSlot, _caughtNotes))
                    {
                        ballotGrabbedEmitter?.PlayAudioOnce();
                    }
                }
            }
        }

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            if (_atTrashCan && !_pickedUpTrashCan)
            {
                _pickedUpTrashCan = true;
                ashtrayPickupEmitter?.PlayAudioOnce();
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
            endText.SetActive(true);
            yield return new WaitForSeconds(endTextDuration);
            if (endAnimator != null) endAnimator.SetActive(true);
            yield return new WaitForSeconds(endAnimationDuration);
            foreach(var note in _caughtNotes)
            {
                var spriteRenderer = note.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.sprite = allNotesCaughtSprite;
                }
                //note.transform.localScale = new Vector3(notesCaughtScale, notesCaughtScale, 1f);
            }
            yield return new WaitForSeconds(timeBeforeSceneLoad);
            SceneLoader.Instance?.ActivatePreloadedScene();
        }
    }
}