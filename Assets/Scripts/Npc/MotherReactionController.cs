using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Npc
{
    public class MotherReactionController : MonoBehaviour
    {
        [Header("Reaction Bubble")]
        [SerializeField] private SpriteRenderer reactionBubbleSpriteRenderer;
        [SerializeField] private List<Sprite> reactionSprites;
        [SerializeField] private float reactionDuration = 1f;
        private int _lastShownIndex = -1;
        private Coroutine _activeReaction;
        
        public void ShowAcceptedReaction()
        {
            // public wrapper in case we split the reactions later
            ShowRandomReaction();
        }
        
        public void ShowRejectedReaction()
        { 
            // public wrapper in case we split the reactions later
            ShowRandomReaction();
        }

        private void ShowRandomReaction(List<Sprite> sourceList = null)
        {
            if(reactionSprites == null || reactionSprites.Count == 0) return;
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
    }
}
