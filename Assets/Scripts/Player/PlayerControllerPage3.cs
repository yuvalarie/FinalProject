using System;
using System.Collections;
using Managers;
using Unity.VisualScripting;
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
        [SerializeField] private Animator letterAnimator1;
        [SerializeField] private Animator letterAnimator2;
        [SerializeField] private GameObject letterObject;
        [SerializeField] private GameObject smallLetterObject;
        
        private int _interactionCount = 0;
        private bool _hasLetterSent;
        private bool _hasLetterOpened;
        private bool _hasLetterChanged;
        
        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            switch (_interactionCount)
            {
                case 1:
                    StartCoroutine(Sequence2Coroutine());
                    break;
                case 2:
                    StartCoroutine(Sequence3Coroutine());
                    break;
                case 3:
                    StartCoroutine(Sequence4Coroutine());
                    break;
                case 4:
                    StartCoroutine(Sequence5Coroutine());
                    break;
                case 5:
                    StartCoroutine(Sequence6Coroutine());
                    break;
                case 6:
                    StartCoroutine(Sequence7Coroutine());
                    break;
                case 7:
                    StartCoroutine(Sequence8Coroutine());
                    break;
            }
        }
        
        private IEnumerator Sequence1Coroutine()
        {
            textBubble1.SetActive(true);
            start.SetActive(false);
            sprite1.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            _interactionCount++;
        }
        
        private IEnumerator Sequence2Coroutine()
        {
            textBubble1.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            textBubble2.SetActive(true);
            sprite1.SetActive(false);
            sprite2.SetActive(true);
            _interactionCount++;
        }
        
        private IEnumerator Sequence3Coroutine()
        {
            textBubble2.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            textBubble3.SetActive(true);
            sprite2.SetActive(false);
            sprite3.SetActive(true);
            hellDoor.SetActive(true);
            _interactionCount++;
        }
        
        private IEnumerator Sequence4Coroutine()
        {
            hellDoor.SetActive(false);
            textBubble3.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            textBubble4.SetActive(true);
            sprite3.SetActive(false);
            sprite4.SetActive(true);
            _interactionCount++;
        }
        
        private IEnumerator Sequence5Coroutine()
        {
            textBubble4.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            textBubble5.SetActive(true);
            sprite4.SetActive(false);
            sprite5.SetActive(true);
            _interactionCount++;
        }
        
        private IEnumerator Sequence6Coroutine()
        {
            if (!_hasLetterSent)
            {
                letterAnimator1.SetTrigger("Send");
                _hasLetterSent = true;
            }
            yield return new WaitForSeconds(3.5f); //change to match animation timing
            letterObject.SetActive(true);
            if (!_hasLetterOpened)
            {
                letterAnimator2.SetTrigger("Open");
                _hasLetterOpened = true;
            }
            yield return new WaitForSeconds(2.5f); //change to match animation timing
            _interactionCount++;
        }
        
        private IEnumerator Sequence7Coroutine()
        {
            if (!_hasLetterChanged)
            {
                letterAnimator2.SetTrigger("Change");
                _hasLetterChanged = true;
            }
            yield return new WaitForSeconds(0.2f); //change to match animation timing
            _interactionCount++;
        }
        
        private IEnumerator Sequence8Coroutine()
        {
            smallLetterObject.SetActive(false);
            textBubble5.SetActive(false);
            letterObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            textBubble6.SetActive(true);
            sprite5.SetActive(false);
            sprite6.SetActive(true);
            endTriggerObject.SetActive(true);
            _interactionCount++;
        }
        

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Start") && _interactionCount == 0) StartCoroutine(Sequence1Coroutine());
            if (other.CompareTag("End") && _interactionCount >= 8)
            {
                SceneLoader.Instance?.ActivatePreloadedScene();
            }
        }
    }
}