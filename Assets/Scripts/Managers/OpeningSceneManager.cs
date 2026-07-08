using UnityEngine;
using UnityEngine.InputSystem;

using UnityEngine.InputSystem.Controls;

using UnityEngine.SceneManagement;

namespace Managers
{
    public class OpeningSceneManager : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("How long to wait before accepting input (in seconds)")]
        public float inputDelay = 0.5f; 
    
        private float timeSinceStart = 0f;
        private bool hasReleasedInitialInput = false;
        
        void Update()
        {
            timeSinceStart += Time.deltaTime;
            if (timeSinceStart < inputDelay)
            {
                return;
            }

            if (!hasReleasedInitialInput)
            {
                if (IsStartInputCurrentlyPressed())
                {
                    return;
                }

                hasReleasedInitialInput = true;
                return;
            }

            if (WasStartInputPressedThisFrame())
            {
                Debug.Log("[OpeningSceneManager] Start input accepted. Loading Page 1.");
                SceneManager.LoadScene("Tutorial");
            }
        }

        private static bool IsStartInputCurrentlyPressed()
        {
            if (Keyboard.current != null && Keyboard.current.anyKey.isPressed)
            {
                return true;
            }

            foreach (Gamepad gamepad in Gamepad.all)
            {
                if (IsPressed(gamepad.buttonSouth) ||
                    IsPressed(gamepad.buttonNorth) ||
                    IsPressed(gamepad.buttonEast) ||
                    IsPressed(gamepad.buttonWest) ||
                    IsPressed(gamepad.startButton) ||
                    IsPressed(gamepad.selectButton) ||
                    IsPressed(gamepad.leftShoulder) ||
                    IsPressed(gamepad.rightShoulder) ||
                    IsPressed(gamepad.leftStickButton) ||
                    IsPressed(gamepad.rightStickButton) ||
                    IsPressed(gamepad.dpad.up) ||
                    IsPressed(gamepad.dpad.down) ||
                    IsPressed(gamepad.dpad.left) ||
                    IsPressed(gamepad.dpad.right))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool WasStartInputPressedThisFrame()
        {
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            {
                return true;
            }

            foreach (Gamepad gamepad in Gamepad.all)
            {
                if (WasPressed(gamepad.buttonSouth) ||
                    WasPressed(gamepad.buttonNorth) ||
                    WasPressed(gamepad.buttonEast) ||
                    WasPressed(gamepad.buttonWest) ||
                    WasPressed(gamepad.startButton) ||
                    WasPressed(gamepad.selectButton) ||
                    WasPressed(gamepad.leftShoulder) ||
                    WasPressed(gamepad.rightShoulder) ||
                    WasPressed(gamepad.leftStickButton) ||
                    WasPressed(gamepad.rightStickButton) ||
                    WasPressed(gamepad.dpad.up) ||
                    WasPressed(gamepad.dpad.down) ||
                    WasPressed(gamepad.dpad.left) ||
                    WasPressed(gamepad.dpad.right))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsPressed(ButtonControl button)
        {
            return button != null && button.isPressed;
        }

        private static bool WasPressed(ButtonControl button)
        {
            return button != null && button.wasPressedThisFrame;
        }
    }
}
