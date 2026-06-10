using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        [Header("Transition Settings")]
        [SerializeField] private CanvasGroup transitionCanvasGroup; 
        [SerializeField] private float fadeDuration = 0.2f; // Fast, punchy fade

        private AsyncOperation preloadedSceneOperation;
        private bool isTransitioning = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// STEP 1: Call this early! This silently streams the next scene into RAM.
        /// </summary>
        public void PreloadScene(string sceneName)
        {
            // Avoid loading multiple scenes at once
            if (preloadedSceneOperation != null) return;
            if (sceneName == null) return;
            
            StartCoroutine(PreloadRoutine(sceneName));
        }

        private IEnumerator PreloadRoutine(string sceneName)
        {
            preloadedSceneOperation = SceneManager.LoadSceneAsync(sceneName);
            // This stops Unity from auto-switching when it hits 100%
            preloadedSceneOperation.allowSceneActivation = false;

            // Wait until it reaches the 90% stall point (fully loaded in memory)
            while (preloadedSceneOperation.progress < 0.9f)
            {
                yield return null;
            }
            
            Debug.Log($"[SceneLoader] {sceneName} is fully loaded in RAM and waiting.");
        }

        /// <summary>
        /// STEP 2: Call this when the player hits the exit. It will feel completely immediate.
        /// </summary>
        public void ActivatePreloadedScene()
        {
            if (preloadedSceneOperation == null)
            {
                Debug.LogWarning("No scene was preloaded! Make sure you called PreloadScene first.");
                return;
            }

            if (isTransitioning) return;
            StartCoroutine(TransitionRoutine());
        }

        private IEnumerator TransitionRoutine()
        {
            isTransitioning = true;

            // 1. Run your quick visual cover (e.g., fade out or slide a comic border)
            //yield return StartCoroutine(Fade(1f));

            // 2. Instantly flip the switch. Because it's preloaded, the swap is immediate.
            AsyncOperation currentTransitionOp = preloadedSceneOperation;
            preloadedSceneOperation = null;
            currentTransitionOp.allowSceneActivation = true;

            // Wait a tiny fraction of a second for Unity to process the activation
            while (!currentTransitionOp.isDone)
            {
                yield return null;
            }
            
            // 3. Uncover the new scene
            //yield return StartCoroutine(Fade(0f));

            isTransitioning = false;
        }

        private IEnumerator Fade(float targetAlpha)
        {
            float speed = 1f / fadeDuration;
            while (!Mathf.Approximately(transitionCanvasGroup.alpha, targetAlpha))
            {
                transitionCanvasGroup.alpha = Mathf.MoveTowards(transitionCanvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
                yield return null;
            }
        }
    }
}