# Debug Hotkeys

Reference for the debug controls implemented in `DebugSceneHotkeys.cs` / `DebugSceneNavigator.cs`
(`Assets/Scripts/Managers/`). These are active in every scene automatically — no setup needed.

## Safety gate

All debug hotkeys require a modifier combo to be held before the action key/button registers,
so they don't trigger accidentally during normal play:

- **Keyboard (Windows):** hold **Ctrl + Alt**
- **Keyboard (Mac):** hold **Control + Option** (physical Ctrl + Alt keys — not Command/⌘)
- **Gamepad:** hold **both triggers** (LT + RT / L2 + R2)

The gamepad gate uses the analog triggers specifically because they aren't bound to anything in
the game's actual gameplay controls (`InputSystem_Actions`) — every face button, shoulder button,
and the d-pad are already used for real gameplay across the various pages/minigames, so triggers
were the only unused controls available for a safe gate.

## Gamepad

| Action | Combo |
|---|---|
| Restart current scene | Hold LT+RT, press **West** (Xbox: X · PlayStation: Square) |
| Next scene (build order) | Hold LT+RT, press **East** (Xbox: B · PlayStation: Circle) |
| Restart whole game | Hold LT+RT, press **North** (Xbox: Y · PlayStation: Triangle) |
| Quit game | Hold LT+RT, press **South** (Xbox: A · PlayStation: Cross) |

There is currently no gamepad equivalent for jumping directly to a numbered scene slot — that's
keyboard-only (see below).

## Keyboard

> **Mac note:** Unity's Input System maps `Ctrl` to the physical **Control** key and `Alt` to the
> physical **Option** key on Mac hardware — **not** Command (⌘). Use Control + Option, not Cmd,
> or the gate won't register.

| Action | Windows | Mac |
|---|---|---|
| Restart current scene | Ctrl + Alt + R | Control + Option + R |
| Next scene (build order) | Ctrl + Alt + N | Control + Option + N |
| Restart whole game | Ctrl + Alt + G | Control + Option + G |
| Quit game | Ctrl + Alt + Q | Control + Option + Q |
| Jump to scene slot 1–10 | Ctrl + Alt + [0–9] | Control + Option + [0–9] |
| Jump to scene slot 11–20 | Ctrl + Alt + Shift + [0–9] | Control + Option + Shift + [0–9] |
| Jump to scene slot 21 | Ctrl + Alt + Shift + Minus (-) | Control + Option + Shift + Minus (-) |

Scene slot 0 → slot 10 (the `0` key maps to slot 10, not slot 0). Slots are defined in
`DebugSceneCatalog.cs`; some slots are reserved placeholders and will log a warning instead of
loading anything if triggered.

## Idle auto-reset (not a manual hotkey)

Separately, `IdleResetWatcher.cs` automatically resets the game to `StartScreen` after a period of
no input (default 90s), for the unattended display build. This is not a hotkey — it fires on its
own timer and only outside the `StartScreen` scene.
