using System.Collections;
using Objects;
using Player;
using UnityEngine;

namespace Managers
{
    public class MiniGame2SceneController : MonoBehaviour
    {
        [Header("Controllers")]
        // The walking controller is active at the start — hand controller starts disabled
        [SerializeField] private PlayerControllerMiniGame2Walk walkController;
        [SerializeField] private PlayerControllerMiniGame2Hand handController;

        [Header("Hand Entry")]
        // The hand GameObject that moves from hidden to starting position
        [SerializeField] private Transform hand;
        // Place these empty GameObjects in the scene to define the two positions
        [SerializeField] private Transform handHiddenPosition;
        [SerializeField] private Transform handStartingPosition;
        [SerializeField] private float handEntryDuration = 1f;
        [SerializeField] private AnimationCurve handEntryCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Stop Trigger")]
        // The trigger collider the player walks into to stop and begin the hand entry
        [SerializeField] private Collider2D stopTrigger;

        [Header("References")]
        [SerializeField] private FriendSpawner friendSpawner;

        [Header("Debug")]
        [SerializeField] private bool skipWalkIn;
        [SerializeField] private bool skipHandEntry;

        private void Start()
        {
            hand.position = handHiddenPosition.position;
            handController.enabled = false;
            DebugStart();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject != walkController.gameObject) return;
            OnPlayerReachedStopPosition();
        }

        private void OnPlayerReachedStopPosition()
        {
            // Lock the player in place and begin the hand entry
            walkController.DisableMovement();
            stopTrigger.enabled = false;
            StartHandEntry();
        }

        private void StartHandEntry()
        {
            if (skipHandEntry)
            {
                OnHandEntryComplete();
                return;
            }
            StartCoroutine(HandEntryRoutine());
        }

        private IEnumerator HandEntryRoutine()
        {
            float elapsed = 0f;
            Vector3 startPos = handHiddenPosition.position;
            Vector3 endPos = handStartingPosition.position;

            while (elapsed < handEntryDuration)
            {
                elapsed += Time.deltaTime;
                float t = handEntryCurve.Evaluate(Mathf.Clamp01(elapsed / handEntryDuration));
                hand.position = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }

            hand.position = endPos;
            OnHandEntryComplete();
        }

        private void OnHandEntryComplete()
        {
            handController.enabled = true;
            friendSpawner.AllFriendsSwiped += OnSwipingComplete;
            friendSpawner.StartSpawning();
        }

        // Placeholder for end-of-swiping sequence.
        // Future: trigger mother reactions and friend exodus here before the hand leaves.
        private void OnSwipingComplete()
        {
            friendSpawner.AllFriendsSwiped -= OnSwipingComplete;
            StartHandExit();
        }

        private void StartHandExit()
        {
            StartCoroutine(HandExitRoutine());
        }

        private IEnumerator HandExitRoutine()
        {
            float elapsed = 0f;
            Vector3 startPos = handStartingPosition.position;
            Vector3 endPos = handHiddenPosition.position;

            while (elapsed < handEntryDuration)
            {
                elapsed += Time.deltaTime;
                float t = handEntryCurve.Evaluate(Mathf.Clamp01(elapsed / handEntryDuration));
                hand.position = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }

            hand.position = endPos;
            OnHandExitComplete();
        }

        private void OnHandExitComplete()
        {
            handController.enabled = false;
            walkController.EnableMovement();
        }

        // Handles debug entry points — skips walk-in and/or hand entry if flagged
        private void DebugStart()
        {
            if (skipWalkIn)
                OnPlayerReachedStopPosition();
        }
    }
}
