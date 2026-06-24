using System;
using System.Collections;
using System.Collections.Generic;
using Audio.AudioEmitters;
using DG.Tweening;
using Managers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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
        [SerializeField, Tooltip("Big Helmet Offset")] private float offSetY;
        [SerializeField, Tooltip("Big Helmet Offset")] private float offSetX;
        [SerializeField] private List<GameObject> availableHelmets;
        [SerializeField, Tooltip("How much higher each additional helmet should sit (e.g., Y = 0.2)")]
        private Vector3 helmetStackOffset = new Vector3(0f, 0.5f, 0f);
        [SerializeField] private Animator leftDoorAnimator;
        [SerializeField] private Animator rightDoorAnimator;
        [SerializeField] private Collider2D leftDoorCollider;
        [SerializeField] private Collider2D rightDoorCollider;
        [SerializeField] private SpriteRenderer rightDoorSpriteRenderer;
        [SerializeField] private Collider2D helmetArea;

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

        [Header("Last frame interaction settings")] 
        [SerializeField] private Collider2D lastFrameTrigger;
        [SerializeField] private GameObject textBubble1;
        [SerializeField] private GameObject textBubble2;

        private bool _atHelmetArea;
        private bool _atPieArea;
        private bool _atDrinkArea;

        private SpriteRenderer _spriteRenderer;
        private float _elevatorOffsetY;
        private int _equippedHelmetCount = 0;
        private bool _hasActivatedLastFrameSequence = false;
        private Vector3 _elevatorStartPosition;
        private int _sortingOrderAtStart;
        private List<GameObject> _equippedHelmets = new List<GameObject>();
        
        protected override void Start()
        {
            base.Start();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _sortingOrderAtStart = _spriteRenderer.sortingOrder;
            _elevatorStartPosition = elevatorTarget.position;
        }

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (_atHelmetArea) HelmetInteraction();
            if (_atDrinkArea) DrinkInteraction();
            if (_atPieArea) PieInteraction();
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
            if (_equippedHelmetCount == 1)
            {
                leftDoorAnimator.SetTrigger(Open);
                rightDoorAnimator.SetTrigger(Open);
                rightDoorCollider.enabled = false;
                leftDoorCollider.enabled = false;
                rightDoorSpriteRenderer.sortingOrder = 10;
            }
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            if (other == helmetArea)
            {
                _atHelmetArea = true;
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
                // transform.localScale = new Vector3(frame6Scale, frame6Scale, 1f);
                // _spriteRenderer.sprite = frame6Sprite;
                CurrentSize = frame6Size;
                SetSize();
                foreach (var helmet in _equippedHelmets)
                {
                    var spriteRenderer = helmet.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null) spriteRenderer.sprite = frame6HelmetSprite;
                    helmet.transform.localScale = frame6HelmetSize;
                    helmet.transform.localPosition = new Vector3(helmet.transform.localPosition.x + offSetX, helmet.transform.localPosition.y + offSetY, helmet.transform.localPosition.z);
                }
            }
            else if (other == frame6To5Trigger)
            {
                // transform.localScale = new Vector3(frame1To6Scale, frame1To6Scale, 1f);
                // _spriteRenderer.sprite = frame1Sprite;
                CurrentSize = frame1Size;
                SetSize();
                foreach (var helmet in _equippedHelmets)
                {
                    var spriteRenderer = helmet.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null) spriteRenderer.sprite = frame1HelmetSprite;
                    helmet.transform.localScale = frame1HelmetSize;
                    helmet.transform.localPosition = new Vector3(helmet.transform.localPosition.x - offSetX, helmet.transform.localPosition.y - offSetY, helmet.transform.localPosition.z);
                }
            }
            
            if (other == lastFrameTrigger && !_hasActivatedLastFrameSequence)
            {
                StartCoroutine(LastFrameSequenceCoroutine());
                _hasActivatedLastFrameSequence = true;
            }
        }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other == elevatorTrigger)
            {
                bool isAwayFromTop = Vector3.Distance(elevatorTarget.position, elevatorTargetPlacement.position) > 0.05f;
                bool isAwayFromBottom = Vector3.Distance(elevatorTarget.position, _elevatorStartPosition) > 0.05f;

                if (isAwayFromTop && isAwayFromBottom)
                {
                    transform.position = new Vector3(elevatorPlacement.position.x, transform.position.y, transform.position.z);
                }
                
                if (MoveInput.y != 0f)
                {
                    float desiredY = transform.position.y + _elevatorOffsetY;
                    float minY = _elevatorStartPosition.y;
                    float maxY = elevatorTargetPlacement.position.y;
                    float clampedY = Mathf.Clamp(desiredY, minY, maxY);
                    elevatorTarget.position = new Vector3(
                        elevatorTarget.position.x, 
                        clampedY,
                        elevatorTarget.position.z
                    );
                    if (desiredY != clampedY)
                    {
                        transform.position = new Vector3(
                            transform.position.x, 
                            clampedY - _elevatorOffsetY, 
                            transform.position.z
                        );
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other == helmetArea)
            {
                _atHelmetArea = false;
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
            textBubble1.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            textBubble2.SetActive(true);
        }
    }
}