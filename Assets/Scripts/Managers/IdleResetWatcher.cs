using Objects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class IdleResetWatcher : MonoBehaviour
    {
        private const string StartSceneName = "StartScreen";

        [Header("Idle Reset")]
        [Tooltip("Seconds of no input before the game resets to the start screen.")]
        [SerializeField] private float idleTimeoutSeconds = 180f;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            InputSystem.onEvent += OnInputEvent;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            InputSystem.onEvent -= OnInputEvent;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == StartSceneName)
            {
                CancelInvoke(nameof(TriggerReset));
                return;
            }

            RearmTimer();
        }

        private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
        {
            if (SceneManager.GetActiveScene().name == StartSceneName)
            {
                return;
            }

            RearmTimer();
        }

        private void RearmTimer()
        {
            CancelInvoke(nameof(TriggerReset));
            Invoke(nameof(TriggerReset), idleTimeoutSeconds);
        }

        private void TriggerReset()
        {
            Debug.Log("[IdleResetWatcher] No input detected. Resetting to start screen.");
            MechanicalCounter.ClearPersistedValues();

            if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.ForceLoadScene(StartSceneName);
            }
            else
            {
                SceneManager.LoadScene(StartSceneName);
            }
        }
    }
}
