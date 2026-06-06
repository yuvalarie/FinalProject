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
        
        [Header("Text Settings")]
        [SerializeField] private GameObject textBubble1;
        [SerializeField] private GameObject textBubble2;
        [SerializeField] private GameObject textBubble3;
        [SerializeField] private GameObject textBubble4;
        [SerializeField] private GameObject textBubble5;
        [SerializeField] private GameObject textBubble6;
        
        [Header("Sequence Settings")]
        [SerializeField] private GameObject hellDoor;
        [SerializeField] private Animator letterAnimator;
        [SerializeField] private GameObject letterObject;
        [SerializeField] private GameObject smallLetterObject;
        
        private int _interactionCount = 0;
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
            }
        }
        
        private IEnumerator Sequence1Coroutine()
        {
            textBubble1.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            _interactionCount++;
        }
        
        private IEnumerator Sequence2Coroutine()
        {
            textBubble1.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            textBubble2.SetActive(true);
            _interactionCount++;
        }
        
        private IEnumerator Sequence3Coroutine()
        {
            textBubble2.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            textBubble3.SetActive(true);
            hellDoor.SetActive(true);
            _interactionCount++;
        }
        
        private IEnumerator Sequence4Coroutine()
        {
            hellDoor.SetActive(false);
            textBubble3.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            textBubble4.SetActive(true);
            _interactionCount++;
        }
        
        private IEnumerator Sequence5Coroutine()
        {
            textBubble4.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            textBubble5.SetActive(true);
            _interactionCount++;
        }
        
        private IEnumerator Sequence6Coroutine()
        {
            letterAnimator.SetTrigger("Open");
            yield return new WaitForSeconds(1.2f); //change to match animation timing
            letterObject.SetActive(true);
            _interactionCount++;
        }
        
        private IEnumerator Sequence7Coroutine()
        {
            smallLetterObject.SetActive(false);
            textBubble5.SetActive(false);
            letterObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            textBubble6.SetActive(true);
            endTriggerObject.SetActive(true);
            _interactionCount++;
        }
        

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Start") && _interactionCount == 0) StartCoroutine(Sequence1Coroutine());
            if (other.CompareTag("End") && _interactionCount >= 7)
            {
                SceneLoader.Instance?.ActivatePreloadedScene();
            }
        }
    }
}