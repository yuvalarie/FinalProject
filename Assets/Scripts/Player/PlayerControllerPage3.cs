using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerControllerPage3 : PlayerControllerBase
    {
        [Header("Text Settings")]
        [SerializeField] private GameObject textBubble;
        [SerializeField] private GameObject textBubble1;
        [SerializeField] private GameObject textBubble2;
        [SerializeField] private GameObject textBubble3;
        [SerializeField] private GameObject textBubble4;
        
        [Header("Sequence Settings")]
        [SerializeField] private GameObject hellDoor;
        [SerializeField] private Animator letterAnimator;
        [SerializeField] private GameObject letterObject;
        [SerializeField] private GameObject doorObject;
        [SerializeField] private GameObject doorHandleObject;
        
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
            }
        }
        
        private IEnumerator Sequence1Coroutine()
        {
            textBubble.SetActive(true);
            textBubble1.SetActive(true);
            yield return new WaitForSeconds(2f);
            hellDoor.SetActive(true);
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
            hellDoor.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            textBubble3.SetActive(true);
            _interactionCount++;
        }
        
        private IEnumerator Sequence4Coroutine()
        {
            //letterAnimator.SetTrigger("Open");
            yield return new WaitForSeconds(0.1f); //change to match animation timing
            letterObject.SetActive(true);
            _interactionCount++;
        }
        
        private IEnumerator Sequence5Coroutine()
        {
            letterObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            doorObject.SetActive(true);
            doorHandleObject.SetActive(false);
            textBubble3.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            textBubble4.SetActive(true);
            _interactionCount++;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Start") && _interactionCount == 0) StartCoroutine(Sequence1Coroutine());
        }
    }
}