using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audio;
using Audio.Voice;
using FMOD.Studio;
using UnityEngine;
using Random = UnityEngine.Random;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Npc
{
    public class MotherReactionController : MonoBehaviour
    {
        [Header("Reaction Bubble")]
        [SerializeField] private SpriteRenderer reactionBubbleSpriteRenderer;
        [SerializeField] private List<Sprite> reactionSprites;
        [SerializeField] private float reactionDuration = 1f;
        [SerializeField] private List<Sprite> acceptedReactionSprites;
        [SerializeField] private List<Sprite> rejectedReactionSprites;

        [Serializable]
        private struct VoicedReactionEntry
        {
            public Sprite sprite;
            public CharacterType character;
            public int lineNumber;
        }
        [SerializeField] private List<VoicedReactionEntry> acceptedVoicedReactions;
        [SerializeField] private List<VoicedReactionEntry> rejectedVoicedReactions;
        [Header("Finished Reactions")]
        [SerializeField, Tooltip("GameObjects for the end of the minigame reactions of the mother, needs to be in order")]
        private List<GameObject> finishedReactionObjects;
        [SerializeField, Tooltip("the index needs to be the same index as in the finished reaction objects"), Min(0)]
        private int reactionIndexWhereFriendsLeave;
        [SerializeField, Tooltip("Minimal time that the reaction is shown"), Min(0.01f)]
        private float finishedReactionCooldownDuration;
        [Header("Anger States")]
        [SerializeField] private SpriteRenderer heldaBodyRenderer;
        [SerializeField, Tooltip("5 sprites in order from least to most angry — index 0 is the default sprite already on the renderer")]
        private List<Sprite> angerStateSprites;

        private int _currentAngerStateIndex = 0;
        
        private Coroutine _finishedReactionCooldown;
        private int _currentFinishedReactionIndex = 0;
        private Action _friendsLeaveAction;
        private int _lastShownIndex = -1;
        private int _lastShownAcceptedIndex = -1;
        private int _lastShownRejectedIndex = -1;
        private EventInstance _activeReactionVoiceInstance;
        private bool _isReactionVoiceInstanceInitialized;
        private Coroutine _activeReaction;
        private bool _finishedAllReactions = false;
        private Action _finishedAllReactionsAction;
        private Action _resumeAction;
        private bool _friendsHaveLeft = false;
        
        private void Start()
        {
            foreach (var obj in finishedReactionObjects)
                obj.SetActive(false);
        }

        public void ShowAcceptedReaction()
        {
            // public wrapper in case we split the reactions later
            // ShowRandomReaction(acceptedReactionSprites);
            ShowRandomVoicedReaction(acceptedVoicedReactions, ref _lastShownAcceptedIndex);
        }

        public void ShowRejectedReaction()
        {
            // public wrapper in case we split the reactions later
            // ShowRandomReaction(rejectedReactionSprites);
            ShowRandomVoicedReaction(rejectedVoicedReactions, ref _lastShownRejectedIndex);
        }

        private void ShowRandomReaction(List<Sprite> sourceList = null)
        {
            if (sourceList == null || sourceList.Count == 0)
            {
                sourceList = reactionSprites;
            }
            if(_activeReaction != null) StopCoroutine(_activeReaction);
            _activeReaction = StartCoroutine(ReactionRoutine(sourceList));
        }

        private IEnumerator ReactionRoutine(List<Sprite> sourceList = null)
        {
            reactionBubbleSpriteRenderer.sprite = PickRandomSprite(sourceList ?? reactionSprites);
            reactionBubbleSpriteRenderer.enabled = true;
            yield return new WaitForSeconds(reactionDuration);
            reactionBubbleSpriteRenderer.enabled = false;
            _activeReaction = null;
        }

        private Sprite PickRandomSprite(List<Sprite> sourceList)
        {
            if(sourceList.Count == 1)
            {
                _lastShownIndex = 0;
                return sourceList[0];
            }
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, sourceList.Count);
            }
            while(randomIndex == _lastShownIndex);
            _lastShownIndex = randomIndex;
            return sourceList[randomIndex];
        }

        private void ShowRandomVoicedReaction(List<VoicedReactionEntry> sourceList, ref int lastShownIndex)
        {
            if (sourceList == null || sourceList.Count == 0) return;
            if (_activeReaction != null) StopCoroutine(_activeReaction);
            StopReactionVoice();
            var entry = PickRandomVoicedEntry(sourceList, ref lastShownIndex);
            _activeReaction = StartCoroutine(VoicedReactionRoutine(entry));
        }

        private IEnumerator VoicedReactionRoutine(VoicedReactionEntry entry)
        {
            reactionBubbleSpriteRenderer.sprite = entry.sprite;
            reactionBubbleSpriteRenderer.enabled = true;
            _activeReactionVoiceInstance = VoiceManager.Instance.PlayLine(entry.character, entry.lineNumber, null);
            _isReactionVoiceInstanceInitialized = true;
            yield return new WaitForSeconds(reactionDuration);
            reactionBubbleSpriteRenderer.enabled = false;
            _activeReaction = null;
        }

        private VoicedReactionEntry PickRandomVoicedEntry(List<VoicedReactionEntry> sourceList, ref int lastShownIndex)
        {
            if (sourceList.Count == 1)
            {
                lastShownIndex = 0;
                return sourceList[0];
            }
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, sourceList.Count);
            }
            while (randomIndex == lastShownIndex);
            lastShownIndex = randomIndex;
            return sourceList[randomIndex];
        }

        private void StopReactionVoice()
        {
            if (_isReactionVoiceInstanceInitialized && _activeReactionVoiceInstance.isValid())
            {
                _activeReactionVoiceInstance.stop(STOP_MODE.IMMEDIATE);
            }
            _isReactionVoiceInstanceInitialized = false;
        }

        public void UpdateAngerState(int swipedCount, int totalCount)
        {
            if (angerStateSprites == null || angerStateSprites.Count == 0) return;
            int stateIndex = Mathf.Clamp((swipedCount - 1) * angerStateSprites.Count / totalCount, 0, angerStateSprites.Count - 1);
            if (stateIndex <= _currentAngerStateIndex) return;
            _currentAngerStateIndex = stateIndex;
            heldaBodyRenderer.sprite = angerStateSprites[stateIndex];
            MusicManager.Instance.AdvanceProgress();
        }

        public void SetFriendsLeaveAction(Action friendsLeaveAction)
        {
            _friendsLeaveAction = friendsLeaveAction;
        }
        
        public void SetFinishedAllReactionsAction(Action finishedAllReactionsAction)
        {
            _finishedAllReactionsAction = finishedAllReactionsAction;
        }

        public void SetResumeAction(Action resumeAction)
        {
            _resumeAction = resumeAction;
        }

        public void StartFinishedReactions()
        {
            StartCoroutine(StartFinishedReactionsRoutine());
        }

        private IEnumerator StartFinishedReactionsRoutine()
        {
            yield return new WaitUntil(() => _activeReaction == null); // wait until any active reaction is done before starting the finished reactions
            TryAdvanceFinishedReaction();
        }


        public void TryAdvanceFinishedReaction()
        {
            // if (_finishedReactionCooldown != null) return;
            if (_finishedReactionCooldown != null || _finishedAllReactions) return;
            if (_currentFinishedReactionIndex >= finishedReactionObjects.Count)
            {
                // if (_finishedAllReactions)
                // {
                //     _resumeAction?.Invoke();
                // }
                // else
                // {
                _finishedAllReactions = true;
                if (!_friendsHaveLeft)
                {
                    _friendsHaveLeft = true;
                    _friendsLeaveAction?.Invoke();
                }
                _finishedAllReactionsAction?.Invoke();
                // }
            }
            else
            {
                _finishedReactionCooldown = StartCoroutine(AdvanceFinishedReactionRoutine());
            }
        }

        public void HideCurrentFinishedReaction()
        {
            if (_currentFinishedReactionIndex > 0)
                finishedReactionObjects[_currentFinishedReactionIndex - 1].SetActive(false);
        }

        public void TryGoBackFinishedReaction()
        {
            if (_finishedReactionCooldown != null) return;
            if (_currentFinishedReactionIndex <= 1) return;
            _finishedReactionCooldown = StartCoroutine(GoBackFinishedReactionRoutine());
        }

        private IEnumerator AdvanceFinishedReactionRoutine()
        {
            if (_currentFinishedReactionIndex > 0)
                finishedReactionObjects[_currentFinishedReactionIndex - 1].SetActive(false);
            if (!_friendsHaveLeft && _currentFinishedReactionIndex == reactionIndexWhereFriendsLeave)
            {
                _friendsHaveLeft = true;
                _friendsLeaveAction?.Invoke();
            }
            finishedReactionObjects[_currentFinishedReactionIndex].SetActive(true);
            _currentFinishedReactionIndex++;
            yield return new WaitForSeconds(finishedReactionCooldownDuration);
            _finishedReactionCooldown = null;
        }

        private IEnumerator GoBackFinishedReactionRoutine()
        {
            finishedReactionObjects[_currentFinishedReactionIndex - 1].SetActive(false);
            _currentFinishedReactionIndex--;
            finishedReactionObjects[_currentFinishedReactionIndex - 1].SetActive(true);
            yield return new WaitForSeconds(finishedReactionCooldownDuration);
            _finishedReactionCooldown = null;
        }
    }
}
