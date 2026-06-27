using System;
using DG.Tweening;
using Objects.Poster;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Sequence = DG.Tweening.Sequence;

namespace Player.Pages
{
    public class PlayerControllerPage13 : PlayerControllerBase
    {
        [Header("Poster Settings")] 
        [SerializeField] private PosterDisplay poster1;
        [SerializeField] private Collider2D poster1Collider;
        [SerializeField] private GameObject rolledPoster1;
        [SerializeField] private GameObject poster1Location;
        
        [SerializeField] private PosterDisplay poster2;
        [SerializeField] private Collider2D poster2Collider;
        [SerializeField] private GameObject rolledPoster2;
        [SerializeField] private GameObject poster2Location;
        
        [SerializeField] private PosterDisplay poster3;
        [SerializeField] private Collider2D poster3Collider;
        [SerializeField] private GameObject rolledPoster3;
        [SerializeField] private GameObject poster3Location;

        [Header("Helda Sequence Settings")] 
        [SerializeField] private GameObject heldaFrame2;
        [SerializeField] private Transform frame2Start;
        [SerializeField] private Transform frame2End;
        [SerializeField] private float frame2Duration;
        [SerializeField] private GameObject handFrame3;
        [SerializeField] private Transform frame3Start;
        [SerializeField] private Transform frame3End;
        [SerializeField] private float frame3Duration;
        [SerializeField] private GameObject note;
        [SerializeField] private GameObject heldaFrame4;
        [SerializeField] private Transform frame4Start;
        [SerializeField] private Transform frame4End;
        [SerializeField] private float frame4Duration;
        [SerializeField] private GameObject handWithNote;
        [SerializeField] private Transform frame5Start;
        [SerializeField] private Transform frame5End;
        [SerializeField] private float frame5Duration;
        [SerializeField] private GameObject noteFrame5;
        [SerializeField] private Transform frame5NoteEnd;
        
        [Header("Bouncy Walk Settings")]
        [SerializeField, Tooltip("How long the movement takes.")] 
        private float duration = 1.5f;
        [SerializeField, Tooltip("How high she bounces on the Y axis while moving.")] 
        private float jumpPower = 0.2f;
        [SerializeField, Tooltip("How many 'hops' she takes to reach the target.")] 
        private int numberOfJumps = 3;
        
        private bool _atPosterCollider1;
        private bool _atPosterCollider2;
        private bool _atPosterCollider3;
        private bool _isPlaced1;
        private bool _isPlaced2;
        private bool _isPlaced3;
        private bool canMove;

        protected override void Start()
        {
            canMove = true;
            base.Start();
            poster1.LoadPoster();
            poster1.gameObject.SetActive(false);
            poster2.LoadPoster();
            poster2.gameObject.SetActive(false);
            poster3.LoadPoster();
            poster3.gameObject.SetActive(false);
        }

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if (_atPosterCollider1)
            {
                _isPlaced1 = true;
                poster1.gameObject.transform.position = poster1Location.transform.position;
                poster1.gameObject.transform.parent = null;
                poster1.gameObject.SetActive(true);
                poster1Collider.gameObject.SetActive(false);
                Destroy(rolledPoster1);
            }
            
            if (_atPosterCollider2)
            {
                _isPlaced2 = true;
                poster2.gameObject.transform.position = poster2Location.transform.position;
                poster2.gameObject.transform.parent = null;
                poster2.gameObject.SetActive(true);
                poster2Collider.gameObject.SetActive(false);
                Destroy(rolledPoster2);
            }
            
            if (_atPosterCollider3)
            {
                _isPlaced3 = true;
                poster3.gameObject.transform.position = poster3Location.transform.position;
                poster3.gameObject.transform.parent = null;
                poster3.gameObject.SetActive(true);
                poster3Collider.gameObject.SetActive(false);
                Destroy(rolledPoster3);
            }
        }

        protected override void HandleMovement()
        {
            if (canMove) base.HandleMovement();
            else Rb.linearVelocity = Vector2.zero;
        }

        private void HeldaSequence()
        {
            canMove = false;
            Rb.linearVelocity = Vector2.zero;

            // Create a new sequence
            Sequence heldaSeq = DOTween.Sequence();

            // 1. Helda Frame 2 Movement
            heldaFrame2.transform.position = frame2Start.position;
            heldaSeq.Append(heldaFrame2.transform.DOJump(frame2End.position, jumpPower, numberOfJumps, frame2Duration)
                .SetEase(Ease.Linear));

            // 2. Hand Frame 3 Movement
            handFrame3.transform.position = frame3Start.position;
            heldaSeq.Append(handFrame3.transform.DOMove(frame3End.position, frame3Duration));
    
            // Callback: Parenting the note
            heldaSeq.AppendCallback(() => note.transform.parent = handFrame3.transform);
    
            // 3. Hand Frame 3 Return
            heldaSeq.Append(handFrame3.transform.DOMove(frame3Start.position, frame3Duration));

            // 4. Helda Frame 4 Movement
            heldaFrame4.transform.position = frame4Start.position;
            // Note: Use heldaFrame4 here, not heldaFrame2!
            heldaSeq.Append(heldaFrame4.transform.DOJump(frame4End.position, jumpPower, numberOfJumps, frame4Duration)
                .SetEase(Ease.Linear));

            // 5. Hand Frame 5 Movement
            handWithNote.transform.position = frame5Start.position;
            heldaSeq.Append(handWithNote.transform.DOMove(frame5End.position, frame5Duration));

            // Callback: Unparent note
            heldaSeq.AppendCallback(() => noteFrame5.transform.parent = null);

            // 6. Note falling
            heldaSeq.Append(noteFrame5.transform.DOMove(frame5NoteEnd.position, frame5Duration));

            // 7. Hand return
            heldaSeq.Append(handWithNote.transform.DOMove(frame5Start.position, frame5Duration));

            // Final callback to re-enable movement
            heldaSeq.OnComplete(() => canMove = true);
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            if (other == poster1Collider)
            {
                _atPosterCollider1 = true;
                rolledPoster1.SetActive(false);
                poster1.gameObject.SetActive(true);
            }
            if (other == poster2Collider)
            {
                _atPosterCollider2 = true;
                rolledPoster2.SetActive(false);
                poster2.gameObject.SetActive(true);
            }
            if (other == poster3Collider)
            {
                _atPosterCollider3 = true;
                rolledPoster3.SetActive(false);
                poster3.gameObject.SetActive(true);
            }

            if (other.CompareTag("Helda"))
            {
                HeldaSequence();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other == poster1Collider)
            {
                _atPosterCollider1 = false;
                rolledPoster1.SetActive(true);
                if (!_isPlaced1) poster1.gameObject.SetActive(false);
            }
            if (other == poster2Collider)
            {
                _atPosterCollider2 = false;
                rolledPoster2.SetActive(true);
                if (!_isPlaced2) poster2.gameObject.SetActive(false);
            }
            if (other == poster3Collider)
            {
                _atPosterCollider3 = false;
                rolledPoster3.SetActive(true);
                if (!_isPlaced3) poster3.gameObject.SetActive(false);
            }
        }
    }
}