using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Managers
{
    public class TutorialManager : MonoBehaviour
    {
        [Serializable]
        private class TutorialPair
        {
            [SerializeField] private GameObject textBubble;
            [SerializeField] private GameObject pairedObject;

            public void SetActive(bool isActive)
            {
                if (textBubble != null) textBubble.SetActive(isActive);
                if (pairedObject != null) pairedObject.SetActive(isActive);
            }
        }

        private enum TutorialStep
        {
            WaitingForAutomaticSecondPair,
            WaitingForForward,
            WaitingForBackward,
            WaitingForRegularTextSequence,
            RegularTextSequence,
            Complete
        }

        [Header("Tutorial Pairs")]
        [SerializeField]
        private TutorialPair[] tutorialPairs =
        {
            new TutorialPair(),
            new TutorialPair(),
            new TutorialPair(),
            new TutorialPair(),
            new TutorialPair(),
            new TutorialPair(),
            new TutorialPair(),
        };

        [Header("Timing")]
        [SerializeField] private float timeBeforeSecondPair = 3f;

        private InputSystem_Actions inputActions;
        private TutorialStep currentStep;
        private int regularSequenceIndex;

        private void Awake()
        {
            inputActions = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            inputActions.Text.Forward.performed += OnTextForward;
            inputActions.Text.Backward.performed += OnTextBackward;
            inputActions.Text.Enable();
        }

        private void Start()
        {
            HideAllPairs();
            ShowPair(0);
            currentStep = TutorialStep.WaitingForAutomaticSecondPair;
            StartCoroutine(ShowSecondPairAfterDelay());
        }

        private void OnDisable()
        {
            if (inputActions == null) return;

            inputActions.Text.Forward.performed -= OnTextForward;
            inputActions.Text.Backward.performed -= OnTextBackward;
            inputActions.Text.Disable();
        }

        private void OnDestroy()
        {
            inputActions?.Dispose();
        }

        private IEnumerator ShowSecondPairAfterDelay()
        {
            yield return null;
            SceneLoader.Instance.PreloadScene("Page 1");

            bool inputReceived = false;
            IDisposable inputListener = InputSystem.onAnyButtonPress.CallOnce(_ => inputReceived = true);

            float elapsed = 0f;
            while (elapsed < timeBeforeSecondPair && !inputReceived)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            inputListener.Dispose();

            ShowPair(1);
            currentStep = TutorialStep.WaitingForForward;
        }

        private void OnTextForward(InputAction.CallbackContext context)
        {
            if (currentStep == TutorialStep.Complete)
            {
                SceneLoader.Instance.ActivatePreloadedScene();
            }
            if (currentStep == TutorialStep.WaitingForForward)
            {
                ShowPair(2);
                currentStep = TutorialStep.WaitingForBackward;
                return;
            }

            if (currentStep == TutorialStep.WaitingForRegularTextSequence)
            {
                StartRegularTextSequence();
                return;
            }

            if (currentStep != TutorialStep.RegularTextSequence) return;

            if (regularSequenceIndex >= 6)
            {
                currentStep = TutorialStep.Complete;
                return;
            }

            regularSequenceIndex++;
            ShowPair(regularSequenceIndex);
        }

        private void OnTextBackward(InputAction.CallbackContext context)
        {
            if (currentStep == TutorialStep.Complete)
            {
                SceneLoader.Instance.ActivatePreloadedScene();
            }
            if (currentStep == TutorialStep.WaitingForBackward)
            {
                ShowPair(3);
                currentStep = TutorialStep.WaitingForRegularTextSequence;
                return;
            }

            if (currentStep != TutorialStep.RegularTextSequence || regularSequenceIndex <= 4) return;

            regularSequenceIndex--;
            ShowPair(regularSequenceIndex);
        }

        private void StartRegularTextSequence()
        {
            regularSequenceIndex = 4;
            currentStep = TutorialStep.RegularTextSequence;
            ShowPair(regularSequenceIndex);
        }

        private void ShowPair(int index)
        {
            if (!HasPair(index)) return;

            HideAllPairs();
            tutorialPairs[index].SetActive(true);
        }

        private void HideAllPairs()
        {
            if (tutorialPairs == null) return;

            for (int i = 0; i < tutorialPairs.Length; i++)
            {
                tutorialPairs[i]?.SetActive(false);
            }
        }

        private bool HasPair(int index)
        {
            return tutorialPairs != null && index >= 0 && index < tutorialPairs.Length && tutorialPairs[index] != null;
        }
    }
}






