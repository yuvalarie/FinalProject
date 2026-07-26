using DebugTools;
using Objects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class IdleResetWatcher : MonoBehaviour
    {
        private const string StartSceneName = "StartScreen";

        [Header("Idle Reset")]
        [Tooltip("Seconds of no input before the game resets to the start screen.")]
        [SerializeField] private float idleTimeoutSeconds = 180f;

        [Tooltip("Minimum input magnitude (0-1) required to count as real input. Filters out analog stick drift and button noise from worn controllers.")]
        [SerializeField] private float idleInputThreshold = 0.3f;

        private InputSystem_Actions _inputActions;

        private void Awake()
        {
            _inputActions = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;

            _inputActions.Game.MoveRight.performed += OnActionPerformed;
            _inputActions.Game.MoveLeft.performed += OnActionPerformed;
            _inputActions.Game.MoveUp.performed += OnActionPerformed;
            _inputActions.Game.MoveDown.performed += OnActionPerformed;
            _inputActions.Game.Interact.performed += OnActionPerformed;
            _inputActions.Game.Trans.performed += OnActionPerformed;
            _inputActions.Text.Forward.performed += OnActionPerformed;
            _inputActions.Text.Backward.performed += OnActionPerformed;

            _inputActions.Game.Enable();
            _inputActions.Text.Enable();

            DebugSceneNavigator.OnDebugHotkeyPressed += RearmTimer;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            _inputActions.Game.MoveRight.performed -= OnActionPerformed;
            _inputActions.Game.MoveLeft.performed -= OnActionPerformed;
            _inputActions.Game.MoveUp.performed -= OnActionPerformed;
            _inputActions.Game.MoveDown.performed -= OnActionPerformed;
            _inputActions.Game.Interact.performed -= OnActionPerformed;
            _inputActions.Game.Trans.performed -= OnActionPerformed;
            _inputActions.Text.Forward.performed -= OnActionPerformed;
            _inputActions.Text.Backward.performed -= OnActionPerformed;

            _inputActions.Game.Disable();
            _inputActions.Text.Disable();

            DebugSceneNavigator.OnDebugHotkeyPressed -= RearmTimer;
        }

        private void OnDestroy()
        {
            _inputActions?.Dispose();
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

        private void OnActionPerformed(InputAction.CallbackContext context)
        {
            if (SceneManager.GetActiveScene().name == StartSceneName)
            {
                return;
            }

            if (Mathf.Abs(context.ReadValue<float>()) < idleInputThreshold)
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
