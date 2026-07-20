using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace DebugTools
{
    public static class DebugSceneHotkeys
    {
        public const int MaxDirectSceneSlot = 21;

        public static bool RestartScenePressed(Keyboard keyboard)
        {
            return (keyboard != null && HasDebugModifiers(keyboard) && WasPressed(keyboard.rKey))
                   || AnyGamepadButtonPressed(gamepad => gamepad.buttonWest);
        }

        public static bool NextScenePressed(Keyboard keyboard)
        {
            return (keyboard != null && HasDebugModifiers(keyboard) && WasPressed(keyboard.nKey))
                   || AnyGamepadButtonPressed(gamepad => gamepad.buttonEast);
        }

        public static bool RestartGamePressed(Keyboard keyboard)
        {
            return (keyboard != null && HasDebugModifiers(keyboard) && WasPressed(keyboard.gKey))
                   || AnyGamepadButtonPressed(gamepad => gamepad.buttonNorth);
        }

        public static bool QuitGamePressed(Keyboard keyboard)
        {
            return (keyboard != null && HasDebugModifiers(keyboard) && WasPressed(keyboard.qKey))
                   || AnyGamepadButtonPressed(gamepad => gamepad.buttonSouth);
        }

        public static bool TryReadSceneSlot(Keyboard keyboard, out int sceneSlot)
        {
            sceneSlot = 0;

            if (keyboard == null || !HasDebugModifiers(keyboard))
            {
                return false;
            }

            bool shiftHeld = IsShiftHeld(keyboard);
            int digit = ReadPressedDigit(keyboard);
            if (digit < 0)
            {
                if (shiftHeld && WasPressed(keyboard.minusKey))
                {
                    sceneSlot = 21;
                    return true;
                }

                return false;
            }

            sceneSlot = shiftHeld ? digit + 10 : digit;
            return sceneSlot >= 1 && sceneSlot <= MaxDirectSceneSlot;
        }

        private static bool HasDebugModifiers(Keyboard keyboard)
        {
            return IsControlHeld(keyboard) && IsAltHeld(keyboard);
        }

        private static bool HasDebugModifiers(Gamepad gamepad)
        {
            return gamepad.leftTrigger.isPressed && gamepad.rightTrigger.isPressed;
        }

        private static bool IsControlHeld(Keyboard keyboard)
        {
            return keyboard.leftCtrlKey.isPressed || keyboard.rightCtrlKey.isPressed;
        }

        private static bool IsAltHeld(Keyboard keyboard)
        {
            return keyboard.leftAltKey.isPressed || keyboard.rightAltKey.isPressed;
        }

        private static bool IsShiftHeld(Keyboard keyboard)
        {
            return keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed;
        }

        private static int ReadPressedDigit(Keyboard keyboard)
        {
            if (WasPressed(keyboard.digit1Key)) return 1;
            if (WasPressed(keyboard.digit2Key)) return 2;
            if (WasPressed(keyboard.digit3Key)) return 3;
            if (WasPressed(keyboard.digit4Key)) return 4;
            if (WasPressed(keyboard.digit5Key)) return 5;
            if (WasPressed(keyboard.digit6Key)) return 6;
            if (WasPressed(keyboard.digit7Key)) return 7;
            if (WasPressed(keyboard.digit8Key)) return 8;
            if (WasPressed(keyboard.digit9Key)) return 9;
            if (WasPressed(keyboard.digit0Key)) return 10;

            return -1;
        }

        private static bool WasPressed(ButtonControl key)
        {
            return key.wasPressedThisFrame;
        }

        private static bool AnyGamepadButtonPressed(System.Func<Gamepad, ButtonControl> getButton)
        {
            foreach (Gamepad gamepad in Gamepad.all)
            {
                if (HasDebugModifiers(gamepad) && WasPressed(getButton(gamepad)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
