using System.Collections;
using Npc;
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
        [SerializeField] private MotherReactionController motherController;

        [Header("End Reminder")]
        [SerializeField, Tooltip("Bubble to show if the player does not leave after the end sequence finishes.")]
        private GameObject delayedEndTextBubble;
        [SerializeField, Min(0f), Tooltip("How many seconds to wait after the end sequence finishes before showing the delayed bubble.")]
        private float delayedEndTextBubbleDelay = 3f;

        [Header("Debug")]
        [SerializeField] private bool skipWalkIn;
        [SerializeField] private bool skipHandEntry;

        private bool _hasCompletedHandExit = false;
        private bool _hasEnteredStopTrigger = false;
        private Coroutine _delayedEndTextBubbleRoutine;

        private void Start()
        {
            hand.position = handHiddenPosition.position;
            handController.DisableSwiping();
            if (delayedEndTextBubble != null) delayedEndTextBubble.SetActive(false);
            friendSpawner.FriendSwiped += OnFriendSwiped;
            DebugStart();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject != walkController.gameObject) return;
            if (_hasEnteredStopTrigger) return;
            _hasEnteredStopTrigger = true;
            // if (_hasCompletedHandExit)
            //     OnPlayerReEnteredStopTrigger();
            // else
            //     OnPlayerReachedStopPosition();
            if (_hasCompletedHandExit) return; // one-shot: scene already finished, ignore further entries
            OnPlayerReachedStopPosition();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject != walkController.gameObject) return;
            _hasEnteredStopTrigger = false;
        }

        private void OnPlayerReEnteredStopTrigger()
        {
            walkController.DisableMovement();
            motherController.SetResumeAction(OnReEntryResume);
            handController.EnableNavigationMode(motherController.TryGoBackFinishedReaction, motherController.TryAdvanceFinishedReaction);
        }

        private void OnReEntryResume()
        {
            walkController.EnableMovement();
            handController.DisableNavigationMode();
        }

        private void OnFriendSwiped(int swipedCount, int totalCount)
        {
            motherController.UpdateAngerState(swipedCount, totalCount);
        }

        private void OnPlayerReachedStopPosition()
        {
            walkController.DisableMovement();
            StartHandEntry();
        }

        private void StartHandEntry()
        {
            if (skipHandEntry)
            {
                OnHandEntryComplete();
                hand.position = handStartingPosition.position;
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
            // handController.enabled = true;
            handController.EnableSwiping();
            friendSpawner.AllFriendsSwiped += OnSwipingComplete;
            // friendSpawner.StartSpawning();
        }

        // Placeholder for end-of-swiping sequence.
        // Future: trigger mother reactions and friend exodus here before the hand leaves.
        private void OnSwipingComplete()
        {
            friendSpawner.AllFriendsSwiped -= OnSwipingComplete;
            friendSpawner.FriendSwiped -= OnFriendSwiped;
            motherController.SetFriendsLeaveAction(friendSpawner.TellFriendsToLeave);
            motherController.SetFinishedAllReactionsAction(StartHandExit);
            motherController.StartFinishedReactions();
            handController.DisableSwiping();
            handController.EnableNavigationMode(motherController.TryGoBackFinishedReaction, motherController.TryAdvanceFinishedReaction);
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
            handController.DisableSwiping();
            handController.DisableNavigationMode();
            walkController.EnableMovement();
            motherController.HideCurrentFinishedReaction();
            _hasCompletedHandExit = true;
            StartDelayedEndTextBubbleCountdown();
        }

        // Handles debug entry points — skips walk-in and/or hand entry if flagged
        private void StartDelayedEndTextBubbleCountdown()
        {
            if (_delayedEndTextBubbleRoutine != null) StopCoroutine(_delayedEndTextBubbleRoutine);
            _delayedEndTextBubbleRoutine = StartCoroutine(DelayedEndTextBubbleRoutine());
        }

        private IEnumerator DelayedEndTextBubbleRoutine()
        {
            yield return new WaitForSeconds(delayedEndTextBubbleDelay);

            if (walkController != null && !walkController.HasReachedEndTrigger && delayedEndTextBubble != null)
            {
                delayedEndTextBubble.SetActive(true);
            }

            _delayedEndTextBubbleRoutine = null;
        }

        private void DebugStart()
        {
            if (skipWalkIn)
                OnPlayerReachedStopPosition();
        }
    }
}
