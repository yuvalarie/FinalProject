using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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
        [Header("Finished Reactions")]
        [SerializeField, Tooltip("Sprites for the end of the minigame reactions of the mother, needs to be in order that we want to show")] 
        private List<Sprite> finishedReactionSprites;
        [SerializeField, Tooltip("the index needs to be the same index as in the finished reaction sprites"), Min(0)] 
        private int reactionIndexWhereFriendsLeave;
        [SerializeField, Tooltip("Minimal time that the reaction is shown"), Min(0.01f)] 
        private float finishedReactionCooldownDuration;
        
        private Coroutine _finishedReactionCooldown;
        private int _currentFinishedReactionIndex = 0;
        private Action _friendsLeaveAction;
        private int _lastShownIndex = -1;
        private Coroutine _activeReaction;
        private bool _finishedAllReactions = false;
        private Action _finishedAllReactionsAction;
        private bool _friendsHaveLeft = false;
        
        public void ShowAcceptedReaction()
        {
            // public wrapper in case we split the reactions later
            ShowRandomReaction(acceptedReactionSprites);
        }
        
        public void ShowRejectedReaction()
        { 
            // public wrapper in case we split the reactions later
            ShowRandomReaction(rejectedReactionSprites);
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
        
        public void SetFriendsLeaveAction(Action friendsLeaveAction)
        {
            _friendsLeaveAction = friendsLeaveAction;
        }
        
        public void SetFinishedAllReactionsAction(Action finishedAllReactionsAction)
        {
            _finishedAllReactionsAction = finishedAllReactionsAction;
        }

        public void TryAdvanceFinishedReaction()
        {
            if (_finishedReactionCooldown != null || _finishedAllReactions) return; // still in cooldown, can't advance yet, ignore input
            if (_currentFinishedReactionIndex >= finishedReactionSprites.Count)
            {
                _finishedAllReactions = true;
                reactionBubbleSpriteRenderer.enabled = false;
                if (!_friendsHaveLeft)
                {
                    _friendsHaveLeft = true;
                    _friendsLeaveAction?.Invoke();
                }
                _finishedAllReactionsAction?.Invoke();
            }
            else
            {
                _finishedReactionCooldown = StartCoroutine(AdvanceFinishedReactionRoutine());
            }
        }
        
        private IEnumerator AdvanceFinishedReactionRoutine()
        {
            reactionBubbleSpriteRenderer.enabled = false;
            yield return null;
            reactionBubbleSpriteRenderer.sprite = finishedReactionSprites[_currentFinishedReactionIndex];
            if (!_friendsHaveLeft && _currentFinishedReactionIndex == reactionIndexWhereFriendsLeave)
            {
                _friendsHaveLeft = true;
                _friendsLeaveAction?.Invoke();
            }
            _currentFinishedReactionIndex++;
            yield return null;
            reactionBubbleSpriteRenderer.enabled = true;
            yield return new WaitForSeconds(finishedReactionCooldownDuration);
            _finishedReactionCooldown = null;
        }
    }
}
