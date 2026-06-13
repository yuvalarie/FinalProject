using System.Collections;
using Audio;
using UnityEngine;
using DG.Tweening;
using FMODUnity;

namespace Objects 
{
    public class PipeLettersAnimator : MonoBehaviour
    {
        [Header("The Letters")]
        [Tooltip("Drag your 3 letter GameObjects here.")]
        [SerializeField] private Transform[] letters;
        [SerializeField] private SpriteRenderer[] lettersSpriteRenderers;

        [Header("Top Pipe Locations")]
        [SerializeField] private Transform topPipeStart;
        [SerializeField] private Transform topPipeEnd;

        [Header("Bottom Pipe Locations")]
        [SerializeField] private Transform bottomPipeStart;
        [SerializeField] private Transform bottomPipeEnd;

        [Header("Randomization Settings")]
        [Tooltip("Minimum time (in seconds) it takes to travel the pipe.")]
        [SerializeField] private float minDuration = 1.5f;
        [Tooltip("Maximum time (in seconds) it takes to travel the pipe.")]
        [SerializeField] private float maxDuration = 3.5f;
        
        [Tooltip("Minimum wait time before a letter starts moving.")]
        [SerializeField] private float minDelay = 0f;
        [Tooltip("Maximum wait time before a letter starts moving (creates distance between them).")]
        [SerializeField] private float maxDelay = 1.5f;
        
        [Header("Anti-Overlap Settings")]
        [Tooltip("Minimum wait time between the two letters sharing the same pipe.")]
        [SerializeField] private float minSeparationTime = 1.0f;

        [Header("Looping")]
        [SerializeField] private bool loopContinuously = true;
        [SerializeField] private float pauseBetweenLoops = 1f;
        
        // Audio
        private EventReference _letterSoundReference;

        private void Start()
        {
            _letterSoundReference = FMODEvents.Instance.GetEventReferenceByName(AudioEventNames.LettersInTubes);
            if (letters.Length != 3)
            {
                Debug.LogWarning("Please assign exactly 3 letters in the Inspector!");
                return;
            }
            
            SendLettersThroughPipes();
        }

        public void SendLettersThroughPipes()
        {
            bool topGetsTwo = Random.value > 0.5f;
            float longestTotalTime = 0f;
            float globalDelayCounter = 0f;
            float lastTopDuration = 0f;
            float lastBottomDuration = 0f;

            for (int i = 0; i < letters.Length; i++)
            {
                Transform currentLetter = letters[i];
                Transform startPoint;
                Transform endPoint;
                
                bool isGoingToTopPipe = topGetsTwo ? (i < 2) : (i >= 2);

                if (isGoingToTopPipe)
                {
                    startPoint = topPipeStart;
                    endPoint = topPipeEnd;
                    lettersSpriteRenderers[i].sortingOrder = 11;
                }
                else
                {
                    startPoint = bottomPipeStart;
                    endPoint = bottomPipeEnd;
                    lettersSpriteRenderers[i].sortingOrder = 8;
                }

                float duration = Random.Range(minDuration, maxDuration);
                float delay = globalDelayCounter + Random.Range(minDelay, maxDelay);
                globalDelayCounter = delay + minSeparationTime;
                
                if (isGoingToTopPipe)
                {
                    if (lastTopDuration > 0f && duration < lastTopDuration)
                    {
                        duration = lastTopDuration + Random.Range(0f, 0.5f);
                    }
                    lastTopDuration = duration; 
                }
                else
                {
                    if (lastBottomDuration > 0f && duration < lastBottomDuration)
                    {
                        duration = lastBottomDuration + Random.Range(0f, 0.5f);
                    }
                    lastBottomDuration = duration; 
                }
                
                if (delay + duration > longestTotalTime)
                {
                    longestTotalTime = delay + duration;
                }

                currentLetter.position = startPoint.position;
                currentLetter.DOMove(endPoint.position, duration)
                    .SetDelay(delay)
                    .SetEase(Ease.Linear);

                StartCoroutine(PlayLetterAudioDelayed(delay, duration));
            }

            if (loopContinuously)
            {
                Invoke(nameof(SendLettersThroughPipes), longestTotalTime + pauseBetweenLoops);
            }
        }

        private void PlayLetterAudio(float duration)
        {
            float t = (duration - minDuration) / (maxDuration - minDuration);
            float pitchAndVolume = Mathf.Clamp(1f - 2f * t, -1f, 1f);
            AudioManager.Instance.PlayOneShot(_letterSoundReference, transform.position, ("LetterTubePitch", pitchAndVolume), ("LetterTubeVolume", pitchAndVolume));
        }

        private IEnumerator PlayLetterAudioDelayed(float delay, float duration)
        {
            yield return new WaitForSeconds(delay);
            PlayLetterAudio(duration);
        }
    }
}