using System;
using System.Collections;
using Audio.AudioEmitters;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerControllerPage3 : PlayerControllerBase
    {
        [SerializeField] private GameObject endTriggerObject;
        
        [Header("Text Settings")]
        [SerializeField] private GameObject textBubble1;
        [SerializeField] private GameObject textBubble2;
        [SerializeField] private GameObject textBubble3;
        [SerializeField] private GameObject textBubble4;
        [SerializeField] private GameObject textBubble5;
        [SerializeField] private GameObject textBubble6;

        [Header("Sprite Settings")] 
        [SerializeField] private GameObject start;
        [SerializeField] private GameObject sprite1;
        [SerializeField] private GameObject sprite2;
        [SerializeField] private GameObject sprite3;
        [SerializeField] private GameObject sprite4;
        [SerializeField] private GameObject sprite5;
        [SerializeField] private GameObject sprite6;
        
        [Header("Sequence Settings")]
        [SerializeField] private GameObject hellDoor;
        [SerializeField] private AudioEmitterBase hellDoorAudioEmitter;
        [SerializeField] private Animator letterAnimator1;
        [SerializeField] private Animator letterAnimator2;
        [SerializeField] private GameObject letterObject;
        [SerializeField] private GameObject smallLetterObject;
        [SerializeField] private GameObject openLetter1;
        [SerializeField] private GameObject openLetter2;
        [SerializeField, Tooltip("How many seconds to wait before popping the final text bubble if they don't leave.")] 
        private float lastTextDelay = 3f;
        
        private int _interactionCount = 0;
        
        private bool _hasLetterSent;
        private bool _hasLetterOpened;
        private bool _hasLetterChanged;
        
        private bool _isSequenceActive = false;
        private bool _isAnimating = false;
        private bool _axisInUse = false;
        
        private Coroutine _currentSequenceRoutine;

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
        }

        protected override void HandleMovement()
        {
            if (_isSequenceActive)
            {
                Rb.linearVelocity = Vector2.zero;
                return;
            }
            
            base.HandleMovement();
        }

        private void Update()
        {
            if (!_isSequenceActive) return;

            if (Mathf.Abs(MoveInput.x) < 0.1f)
            {
                _axisInUse = false;
            }

            if (_isAnimating || _axisInUse) return;

            if (MoveInput.x > 0.5f && _interactionCount < 8)
            {
                _axisInUse = true;
                _interactionCount++;
                if (_currentSequenceRoutine != null) StopCoroutine(_currentSequenceRoutine);
                _currentSequenceRoutine = StartCoroutine(TransitionToState(_interactionCount, true));
            }
            else if (MoveInput.x < -0.5f && _interactionCount > 0)
            {
                _axisInUse = true;
                _interactionCount--;
                if (_currentSequenceRoutine != null) StopCoroutine(_currentSequenceRoutine);
                _currentSequenceRoutine = StartCoroutine(TransitionToState(_interactionCount, false));
            }
        }

        private IEnumerator TransitionToState(int targetState, bool isMovingForward)
        {
            _isAnimating = true;

            textBubble1.SetActive(false); textBubble2.SetActive(false); textBubble3.SetActive(false);
            textBubble4.SetActive(false); textBubble5.SetActive(false); textBubble6.SetActive(false);
            start.SetActive(false); sprite1.SetActive(false); sprite2.SetActive(false);
            sprite3.SetActive(false); sprite4.SetActive(false); sprite5.SetActive(false); sprite6.SetActive(false);
            hellDoor.SetActive(false); hellDoorAudioEmitter.StopAudio();
            letterObject.SetActive(false); endTriggerObject.SetActive(false);
            openLetter1.SetActive(false); openLetter2.SetActive(false);
            smallLetterObject.SetActive(false);


            switch (targetState)
            {
                case 0:
                    start.SetActive(true);
                    _isSequenceActive = false;
                    break;
                
                case 1:
                    sprite1.SetActive(true);
                    textBubble1.SetActive(true);
                    break;
                
                case 2:
                    sprite2.SetActive(true);
                    textBubble2.SetActive(true);
                    break;
                
                case 3:
                    sprite3.SetActive(true);
                    hellDoor.SetActive(true);
                    hellDoorAudioEmitter.StartAudio();
                    textBubble3.SetActive(true);
                    break;
                
                case 4:
                    sprite4.SetActive(true);
                    textBubble4.SetActive(true);
                    break;
                
                case 5:
                    sprite5.SetActive(true);
                    textBubble5.SetActive(true);
                    SceneLoader.Instance?.PreloadScene(nextSceneName);
                    break;
                
                case 6:
                    sprite5.SetActive(true);
                    textBubble5.SetActive(true);

                    if (isMovingForward)
                    {
                        smallLetterObject.SetActive(true);
                        if (!_hasLetterSent) { letterAnimator1.SetTrigger("Send"); _hasLetterSent = true; yield return new WaitForSeconds(3.5f);}
                        letterObject.SetActive(true);
                        
                        if (!_hasLetterOpened) { letterAnimator2.SetTrigger("Open"); _hasLetterOpened = true; yield return new WaitForSeconds(2.5f); }
                        openLetter1.SetActive(true);
                        smallLetterObject.SetActive(false);
                    }
                    else
                    {
                        openLetter1.SetActive(true);
                        smallLetterObject.SetActive(false);
                    }
                    break;
                
                case 7:
                    sprite5.SetActive(true);
                    textBubble5.SetActive(true);
                    
                    if (isMovingForward)
                    {
                        letterObject.SetActive(false);
                        openLetter2.SetActive(true);
                    }
                    else
                    {
                        openLetter1.SetActive(true);
                    }
                    break;
                
                case 8:
                    sprite6.SetActive(true);
                    endTriggerObject.SetActive(true);
                    
                    _isSequenceActive = false; 

                    if (isMovingForward)
                    {
                        yield return new WaitForSeconds(lastTextDelay);
                        
                        if (_interactionCount == 8)
                        {
                            textBubble6.SetActive(true);
                        }
                    }
                    else
                    {
                        openLetter2.SetActive(true);
                    }
                    break;
            }

            _isAnimating = false;
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Start") && !_isSequenceActive && _interactionCount == 0)
            {
                _isSequenceActive = true;
                _interactionCount = 1;
                if (Mathf.Abs(MoveInput.x) > 0.1f)
                {
                    _axisInUse = true;
                }
                if (_currentSequenceRoutine != null) StopCoroutine(_currentSequenceRoutine);
                _currentSequenceRoutine = StartCoroutine(TransitionToState(_interactionCount, true));
            }
            
            if (other.CompareTag("End") && _interactionCount >= 8)
            {
                _interactionCount = 9; 
                SceneLoader.Instance?.ActivatePreloadedScene();
            }
        }
    }
}