using DG.Tweening;
using UnityEngine;

namespace Helpers
{
    public class CloudAnimator : MonoBehaviour
    {
        [Header("The Letters")]
        [Tooltip("Drag your 3 letter GameObjects here.")]
        [SerializeField] private Transform[] clouds;

        [Header("Locations")]
        [SerializeField] private Transform start;
        [SerializeField] private Transform end;

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
        
        private void Start()
        {
            if (clouds.Length == 0)
            {
                Debug.LogWarning("Please assign letters in the Inspector!");
                return;
            }
            
            MoveClouds();
        }

        public void MoveClouds()
        {
            float longestTotalTime = 0f;
            float globalDelayCounter = 0f;
            float lastDuration = 0f;

            for (int i = 0; i < clouds.Length; i++)
            {
                Transform currentCloud = clouds[i];

                float duration = Random.Range(minDuration, maxDuration);
                float delay = globalDelayCounter + Random.Range(minDelay, maxDelay);
                globalDelayCounter = delay + minSeparationTime;

                if (lastDuration > 0f && duration < lastDuration)
                {
                    duration = lastDuration + Random.Range(0f, 0.5f);
                }
                lastDuration = duration;
                
                if (delay + duration > longestTotalTime)
                {
                    longestTotalTime = delay + duration;
                }

                currentCloud.position = start.position;
                currentCloud.DOMove(end.position, duration)
                    .SetDelay(delay)
                    .SetEase(Ease.Linear);

            }

            if (loopContinuously)
            {
                Invoke(nameof(MoveClouds), longestTotalTime + pauseBetweenLoops);
            }
        }
    }
}