using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerControllerPage2 : PlayerControllerBase
    {
        [Header("Size Settings")]
        [Tooltip("The scale factor for the player's size in frame 1.")]
        [SerializeField] private float frame1Scale = 0.5f;
        [Tooltip("The scale factor for the player's size in frame 2-6.")]
        [SerializeField] private float frame2To6Scale = 1f;
        [Tooltip("The scale factor for the player's size in frame 7.")]
        [SerializeField] private float frame7Scale = 1.5f;
        
        [Header("Trigger Settings")]
        [Tooltip("Trigger to transition to frame 2.")]
        [SerializeField] private Collider2D frame1To2Trigger;
        [Tooltip("Trigger to transition to frame 3.")]
        [SerializeField] private Collider2D frame2To3Trigger;
        [Tooltip("Trigger to transition to frame 5.")]
        [SerializeField] private Collider2D frame4To5Trigger;
        [Tooltip("Trigger to transition to frame 6.")]
        [SerializeField] private Collider2D frame5To6Trigger;
        [Tooltip("Trigger to transition to frame 7.")]
        [SerializeField] private Collider2D frame6To7Trigger;
        
        [Header("Mask Settings")]
        [SerializeField] private SpriteMask frame1Mask;
        [SerializeField] private SpriteMask frame2Mask;
        [SerializeField] private SpriteMask frame3Mask;
        [SerializeField] private SpriteMask frame4Mask;
        [SerializeField] private SpriteMask frame5Mask;
        [SerializeField] private SpriteMask frame6Mask;
        [SerializeField] private SpriteMask frame7Mask;
        
        protected override void OnInteraction(InputAction.CallbackContext context)
        {
        }

        private void Start()
        {
            frame2Mask.gameObject.SetActive(false);
            frame3Mask.gameObject.SetActive(false);
            frame4Mask.gameObject.SetActive(false);
            frame5Mask.gameObject.SetActive(false);
            frame6Mask.gameObject.SetActive(false);
            frame7Mask.gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other == frame1To2Trigger)
            {
                transform.localScale = Vector3.one * frame2To6Scale;
                StartCoroutine(MoveToNextFrame(frame1Mask, frame2Mask));
            }
            else if (other == frame2To3Trigger)
            {
                StartCoroutine(MoveToNextFrame(frame2Mask, frame3Mask));
                frame4Mask.gameObject.SetActive(true);
            }
            else if (other == frame4To5Trigger)
            {
                frame3Mask.gameObject.SetActive(false);
                StartCoroutine(MoveToNextFrame(frame4Mask, frame5Mask));
            }
            else if (other == frame5To6Trigger)
            {
                StartCoroutine(MoveToNextFrame(frame5Mask, frame6Mask));
            }
            else if (other == frame6To7Trigger)
            {
                transform.localScale = Vector3.one * frame7Scale;
                StartCoroutine(MoveToNextFrame(frame6Mask, frame7Mask));
            }
        }
        
        private IEnumerator MoveToNextFrame(SpriteMask currentMask, SpriteMask nextMask)
        {
            currentMask.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.2f);
            nextMask.gameObject.SetActive(true);
        }
    }
}