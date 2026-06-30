using System;
using System.Collections;
using System.Collections.Generic;
using Audio.AudioEmitters;
using DG.Tweening;
using Managers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Player
{
    public class PlayerControllerPage2 : PlayerControllerBase
    {
        private static readonly int Open = Animator.StringToHash("Open");
        private static readonly float AudioDelay = 0.5f;

        [Header("Size Settings")]
        [Tooltip("The scale factor for the player's size in frame 1-6.")]
        // [SerializeField] private float frame1To6Scale = 1f;
        // [SerializeField] private Sprite frame1Sprite;
        [SerializeField] private SizeSettings frame1Size;
        [SerializeField] private Vector3 frame1HelmetSize;
        [SerializeField] private Sprite frame1HelmetSprite;
        [Tooltip("The scale factor for the player's size in frame 6.")] 
        // [SerializeField] private float frame6Scale = 1.5f;
        // [SerializeField] private Sprite frame6Sprite;
        [SerializeField] private SizeSettings frame6Size;
        [SerializeField] private Vector3 frame6HelmetSize;
        [SerializeField] private Sprite frame6HelmetSprite;
        
        [Header("Elevator Settings")]
        [SerializeField] private Collider2D elevatorTrigger;
        [SerializeField] private Transform elevatorTarget;
        [SerializeField] private Transform elevatorPlacement;
        [SerializeField] private Transform elevatorTargetPlacement;
        [SerializeField] private float elevatorMoveDuration;
        [SerializeField] private AudioEmitterBase elevatorAudioEmitter;
        
        [Header("Trigger Settings")]
        [SerializeField, Tooltip("The trigger that initiates the transition from frame 5 to 6.")]
        private Collider2D frame5To6Trigger;
        [SerializeField, Tooltip("The trigger that initiates the transition from frame 6 to 5.")]
        private Collider2D frame6To5Trigger;
        [SerializeField, Tooltip("The trigger that initiates the transition from frame 6 to 7.")]
        private Collider2D frame6To7Trigger;
        [SerializeField, Tooltip("The trigger that initiates the transition from frame 7 to 6.")]
        private Collider2D frame7To6Trigger;
        
        [Header("Helmet interaction settings")]
        [SerializeField, Tooltip("The placement for the helmet.")] private Vector3 helmetPlacement;
        [SerializeField, Tooltip("Big Helmet Placement")] private Vector3 bigHelmetPlacement;
        [SerializeField] private List<GameObject> availableHelmets;
        [SerializeField, Tooltip("How much higher each additional helmet should sit (e.g., Y = 0.2)")]
        private Vector3 helmetStackOffset = new Vector3(0f, 0.5f, 0f);
        [SerializeField] private Animator leftDoorAnimator;
        [SerializeField] private Animator rightDoorAnimator;
        [SerializeField] private Collider2D leftDoorCollider;
        [SerializeField] private Collider2D rightDoorCollider;
        [SerializeField] private SpriteRenderer rightDoorSpriteRenderer;
        [SerializeField] private SpriteRenderer rightWallSpriteRenderer;
        [SerializeField] private Collider2D helmetArea;
        [SerializeField] private AudioEmitterBase helmetAudioEmitter;

        [Header("Vending machine interaction settings")] 
        [SerializeField, Tooltip("How far from the exact center the items will scatter.")] 
        private float dropScatterRadius = 0.5f;
        [SerializeField] private float animationDuration;
        [SerializeField] private Collider2D drinkVendingCollider;
        [SerializeField] private GameObject drinkObject;
        [SerializeField] private Transform drinkStartPosition;
        [SerializeField] private Transform drinkEndPosition;
        [SerializeField] private GameObject drinkParent;
        [SerializeField] private Collider2D pieVendingCollider;
        [SerializeField] private GameObject pieObject;
        [SerializeField] private Transform pieStartPosition;
        [SerializeField] private Transform pieEndPosition;
        [SerializeField] private GameObject pieParent;

        [Header("Pipe inteaction settings")] 
        [SerializeField] private float pipeAnimationDuration;
        [SerializeField] private Collider2D pipe1Collider;
        [SerializeField] private GameObject[] pipe1LetterObject;
        [SerializeField] private Transform pipe1StartPosition;
        [SerializeField] private Transform pipe1EndPosition;
        [SerializeField] private GameObject pipe1Parent;
        [SerializeField] private Collider2D pipe2Collider;
        [SerializeField] private GameObject[] pipe2LetterObject;
        [SerializeField] private Transform pipe2StartPosition;
        [SerializeField] private Transform pipe2EndPosition;
        [SerializeField] private GameObject pipe2Parent;

        [Header("Last frame interaction settings")] 
        [SerializeField] private Collider2D lastFrameTrigger;
        [SerializeField] private GameObject textBubble1;
        [SerializeField] private GameObject textBubble2;
        [SerializeField] private float waitForTextBubble1;
        [SerializeField] private float waitForTextBubble2;

        private bool _atHelmetArea;
        private bool _atPieArea;
        private bool _atDrinkArea;
        private bool _atPipeArea1;
        private bool _atPipeArea2;
        private bool _atElevatorArea;
        private bool _isElevatorMoving;

        private float _elevatorOffsetY;
        private int _equippedHelmetCount = 0;
        private bool _hasActivatedLastFrameSequence = false;
        private Vector3 _elevatorStartPosition;
        private int _sortingOrderAtStart;
        private List<GameObject> _equippedHelmets = new List<GameObject>();
        
        protected override void Start()
        {
            base.Start();
            _sortingOrderAtStart = SpriteRenderer.sortingOrder;
            _elevatorStartPosition = elevatorTarget.position;
        }

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (_atHelmetArea) HelmetInteraction();
            if (_atPipeArea1) Pipe1Interaction();
            if (_atPipeArea2) Pipe2Interaction();
            if (_atDrinkArea) DrinkInteraction();
            if (_atPieArea) PieInteraction();
            
            if (_atElevatorArea && !_isElevatorMoving) 
            {
                StartCoroutine(ElevatorRoutine());
            }
        }

        protected override void HandleMovement()
        {
            if (!_isElevatorMoving)
            {
                base.HandleMovement(); // Run standard movement
        
                // If the player is actively pressing left or right, ensure helmets match
                if (Mathf.Abs(MoveInput.x) > 0.1f)
                {
                    RefreshHelmets();
                }
            }
        }

        private void PieInteraction()
        {
            // GameObject newPie = Instantiate(pieObject, pieStartPosition.position, Quaternion.identity,
            //     pieParent.transform);
            // newPie.SetActive(true);
            // Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * dropScatterRadius;
            // Vector3 scatteredTargetPosition = pieEndPosition.position + new Vector3(randomOffset.x, randomOffset.y, 0f);
            //
            // newPie.transform.DOMove(scatteredTargetPosition, animationDuration)
            //     .SetEase(Ease.OutBounce);
            StartCoroutine(PieRoutine());
        }

        private IEnumerator PieRoutine()
        {
            var audioEmitter = pieObject.GetComponent<AudioEmitterBase>();
            if (audioEmitter)
            {
                audioEmitter.SetAudioEventName();
                audioEmitter.PlayAudioOnce();
                yield return new WaitForSeconds(AudioDelay);
            }
            GameObject newPie = Instantiate(pieObject, pieStartPosition.position, Quaternion.identity,
                pieParent.transform);
            newPie.SetActive(true);
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * dropScatterRadius;
            Vector3 scatteredTargetPosition = pieEndPosition.position + new Vector3(randomOffset.x, randomOffset.y, 0f);

            newPie.transform.DOMove(scatteredTargetPosition, animationDuration)
                .SetEase(Ease.OutBounce);
        }

        private void DrinkInteraction()
        {
            // GameObject newDrink = Instantiate(drinkObject, drinkStartPosition.position, Quaternion.identity, drinkParent.transform);
            // newDrink.SetActive(true);
            //
            // Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * dropScatterRadius;
            // Vector3 scatteredTargetPosition = drinkEndPosition.position + new Vector3(randomOffset.x, randomOffset.y, 0f);
            //
            // newDrink.transform.DOMove(scatteredTargetPosition, animationDuration)
            //     .SetEase(Ease.OutBounce);
            StartCoroutine(DrinkRoutine());
        }
        
        private IEnumerator DrinkRoutine()
        {
            var audioEmitter = drinkObject.GetComponent<AudioEmitterBase>();
            if (audioEmitter)
            {
                audioEmitter.SetAudioEventName();
                audioEmitter.PlayAudioOnce();
                yield return new WaitForSeconds(AudioDelay);
            }
            GameObject newDrink = Instantiate(drinkObject, drinkStartPosition.position, Quaternion.identity, drinkParent.transform);
            newDrink.SetActive(true);
            
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * dropScatterRadius;
            Vector3 scatteredTargetPosition = drinkEndPosition.position + new Vector3(randomOffset.x, randomOffset.y, 0f);

            newDrink.transform.DOMove(scatteredTargetPosition, animationDuration)
                .SetEase(Ease.OutBounce);
        }
        
        private void Pipe1Interaction()
        {
            StartCoroutine(Pipe1Routine());
        }
        
        private IEnumerator Pipe1Routine()
        {
            yield return new WaitForSeconds(0f);
            // var audioEmitter = drinkObject.GetComponent<AudioEmitterBase>();
            // if (audioEmitter)
            // {
            //     audioEmitter.SetAudioEventName();
            //     audioEmitter.PlayAudioOnce();
            //     yield return new WaitForSeconds(AudioDelay);
            // }
            int idx = Random.Range(0, pipe1LetterObject.Length);
            GameObject newLetter = Instantiate(pipe1LetterObject[idx], pipe1StartPosition.position, pipe1StartPosition.rotation, pipe1Parent.transform);
            newLetter.SetActive(true);
            
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * dropScatterRadius;
            Vector3 scatteredTargetPosition = pipe1EndPosition.position + new Vector3(randomOffset.x, randomOffset.y, 0f);

            newLetter.transform.DOMove(scatteredTargetPosition, pipeAnimationDuration)
                .SetEase(Ease.OutBounce);
        }
        
        private void Pipe2Interaction()
        {
            StartCoroutine(Pipe2Routine());
        }
        
        private IEnumerator Pipe2Routine()
        {
            yield return new WaitForSeconds(0f);
            // var audioEmitter = drinkObject.GetComponent<AudioEmitterBase>();
            // if (audioEmitter)
            // {
            //     audioEmitter.SetAudioEventName();
            //     audioEmitter.PlayAudioOnce();
            //     yield return new WaitForSeconds(AudioDelay);
            // }
            int idx = Random.Range(0, pipe2LetterObject.Length);
            GameObject newLetter = Instantiate(pipe2LetterObject[idx], pipe2StartPosition.position, pipe2StartPosition.rotation, pipe2Parent.transform);
            newLetter.SetActive(true);
            
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * dropScatterRadius;
            Vector3 scatteredTargetPosition = pipe2EndPosition.position + new Vector3(randomOffset.x, randomOffset.y, 0f);

            newLetter.transform.DOMove(scatteredTargetPosition, pipeAnimationDuration)
                .SetEase(Ease.OutBounce);
        }
        
        private IEnumerator ElevatorRoutine()
        {
            _isElevatorMoving = true;
            
            bool isAtBottom = Vector3.Distance(elevatorTarget.position, _elevatorStartPosition) < 0.5f;
            
            float targetElevatorY = isAtBottom ? elevatorTargetPlacement.position.y : _elevatorStartPosition.y;
            
            transform.position = new Vector3(elevatorPlacement.position.x, elevatorPlacement.position.y, transform.position.z);

            if (Rb != null)
            {
                Rb.linearVelocity = Vector2.zero;
                Rb.isKinematic = true;
            }

            Transform originalParent = transform.parent;
            transform.SetParent(elevatorTarget);
            elevatorAudioEmitter.PlayAudioOnce();
            yield return elevatorTarget.DOMoveY(targetElevatorY, elevatorMoveDuration)
                .SetEase(Ease.InOutSine)
                .WaitForCompletion();

            transform.SetParent(originalParent);
            if (Rb != null)
            {
                Rb.isKinematic = false;
            }

            _isElevatorMoving = false;
        }

        private void HelmetInteraction()
        {
            if (availableHelmets.Count == 0) return;
            int topHelmetIndex = availableHelmets.Count - 1;
            GameObject helmetObject = availableHelmets[topHelmetIndex];
            availableHelmets.RemoveAt(topHelmetIndex);
            helmetObject.transform.SetParent(gameObject.transform);
            helmetObject.transform.localPosition = helmetPlacement + (helmetStackOffset * _equippedHelmetCount);
            var spriteRenderer = helmetObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)            {
                spriteRenderer.sortingOrder = _sortingOrderAtStart + 1 + _equippedHelmetCount;;
            }
            _equippedHelmetCount++;
            _equippedHelmets.Add(helmetObject);
            
            RefreshHelmets();
            
            helmetAudioEmitter.PlayAudioOnce();
            if (_equippedHelmetCount == 1)
            {
                leftDoorAnimator.SetTrigger(Open);
                rightDoorAnimator.SetTrigger(Open);
                rightDoorCollider.enabled = false;
                leftDoorCollider.enabled = false;
                rightDoorSpriteRenderer.sortingOrder = 10;
                rightWallSpriteRenderer.sortingOrder = 9;
            }
        }
        
        private void RefreshHelmets()
        {
            // 1. Check current facing direction from the base script
            bool isFacingLeft = SpriteRenderer != null && SpriteRenderer.flipX;
            float xSign = isFacingLeft ? -1f : 1f;

            // 2. Determine base placement and scale based on current frame size
            bool isBigFrame = CurrentSize == frame6Size;
            Vector3 currentBasePlacement = isBigFrame ? bigHelmetPlacement : helmetPlacement;
            Sprite currentSprite = isBigFrame ? frame6HelmetSprite : frame1HelmetSprite;
            Vector3 currentScale = isBigFrame ? frame6HelmetSize : frame1HelmetSize;

            // 3. Apply to all equipped helmets
            for (int i = 0; i < _equippedHelmets.Count; i++)
            {
                var helmet = _equippedHelmets[i];
        
                var sr = helmet.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = currentSprite;
                    sr.flipX = isFacingLeft; // Flip the sprite
                }

                helmet.transform.localScale = currentScale;

                // Multiply the X position by xSign to shift it to the other side!
                Vector3 targetPlacement = currentBasePlacement + (helmetStackOffset * i);
                helmet.transform.localPosition = new Vector3(targetPlacement.x * xSign, targetPlacement.y, targetPlacement.z);
            }
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            if (other == elevatorTrigger)
            {
                _atElevatorArea = true;
            }
            if (other == helmetArea)
            {
                _atHelmetArea = true;
            }

            if (other == pipe1Collider)
            {
                _atPipeArea1 = true;
            }
            if (other == pipe2Collider)
            {
                _atPipeArea2 = true;
            }
            if (other == pieVendingCollider)
            {
                _atPieArea = true;
            }

            if (other == drinkVendingCollider)
            {
                _atDrinkArea = true;
            }
            if (other == elevatorTrigger)
            {
                var newPosition = new Vector3(elevatorPlacement.position.x, elevatorPlacement.position.y, transform.position.z);
                _elevatorOffsetY = elevatorTarget.position.y - newPosition.y;
            }
            if (other == frame5To6Trigger)
            {
                CurrentSize = frame6Size;
                SetSize();
                RefreshHelmets();
            }
            else if (other == frame6To5Trigger)
            {
                CurrentSize = frame1Size;
                SetSize();
                RefreshHelmets();
            }
            
            if (other == lastFrameTrigger && !_hasActivatedLastFrameSequence)
            {
                StartCoroutine(LastFrameSequenceCoroutine());
                _hasActivatedLastFrameSequence = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other == elevatorTrigger)
            {
                _atElevatorArea = false;
            }
            if (other == helmetArea)
            {
                _atHelmetArea = false;
            }
            if (other == pipe1Collider)
            {
                _atPipeArea1 = false;
            }
            if (other == pipe2Collider)
            {
                _atPipeArea2 = false;
            }

            if (other == pieVendingCollider)
            {
                _atPieArea = false;
            }

            if (other == drinkVendingCollider)
            {
                _atDrinkArea = false;
            }
        }

        private IEnumerator LastFrameSequenceCoroutine()
        {
            yield return new WaitForSeconds(waitForTextBubble1);
            textBubble1.SetActive(true);
            yield return new WaitForSeconds(waitForTextBubble2);
            textBubble2.SetActive(true);
        }
    }
}