using System;
using System.Collections.Generic;
using FMOD.Studio;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Audio.Voice
{
    public class VoiceManager : PersistentMonoSingleton<VoiceManager>
    {
        [SerializeField] private List<VoiceLineId.CharacterNameEntry> characterNameMappings;
        [SerializeField] private List<VoiceLineId.SceneInitialEntry> sceneInitialMappings;

        private string _currentSceneName;
        private Action _stopPreviousCallback;

        protected override void Awake()
        {
            base.Awake();
            VoiceLineId.LoadCharacterNames(characterNameMappings);
            VoiceLineId.LoadSceneInitials(sceneInitialMappings);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            _currentSceneName = SceneManager.GetActiveScene().name;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _currentSceneName = scene.name;
            _stopPreviousCallback = null;
        }

        public EventInstance PlayLine(CharacterType character, int lineNumber, Action stopCallback)
        {
            _stopPreviousCallback?.Invoke();

            string eventName = VoiceLineId.Build(_currentSceneName, character, lineNumber);
            var eventRef = FMODEvents.Instance.GetEventReferenceByName(eventName);
            var instance = AudioManager.Instance.CreateInstance(eventRef);
            instance.start();

            _stopPreviousCallback = stopCallback;
            return instance;
        }
    }
}
