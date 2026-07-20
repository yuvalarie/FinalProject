using System;
using System.Collections.Generic;
using FMOD.Studio;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Audio
{
    public enum MusicArea { Start, Main, MG1, MG2, MG3, MG4_1, MG4_2, End, Breakdown }

    public class MusicManager : PersistentMonoSingleton<MusicManager>
    {
        [Serializable]
        public struct SceneAreaEntry
        {
            public string sceneName;
            public MusicArea area;
        }

        [Header("Scene Mapping")]
        [Tooltip("Maps scene names to their music area. Unmapped scenes default to Main.")]
        [SerializeField] private List<SceneAreaEntry> sceneAreaMappings;

        private Dictionary<string, MusicArea> _sceneAreaDict;
        private EventInstance _musicInstance;
        private int _currentProgress;

        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;
            _sceneAreaDict = new Dictionary<string, MusicArea>();
            foreach (var entry in sceneAreaMappings)
                _sceneAreaDict[entry.sceneName] = entry.area;
        }

        private void Start()
        {
            var eventRef = FMODEvents.Instance.GetEventReferenceByName(AudioEventNames.MusicGame);
            _musicInstance = AudioManager.Instance.CreateInstance(eventRef);
            _musicInstance.start();
            HandleSceneMusic(SceneManager.GetActiveScene().name);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            HandleSceneMusic(scene.name);
        }

        private void HandleSceneMusic(string sceneName)
        {
            var area = _sceneAreaDict.GetValueOrDefault(sceneName, MusicArea.Main);
            SetArea(area);
        }

        private void SetArea(MusicArea area)
        {
            _musicInstance.setParameterByNameWithLabel(AudioEventNames.MusicAreaParam, area.ToString());
            _currentProgress = 0;
            _musicInstance.setParameterByName(AudioEventNames.MusicProgressParam, 0f);
        }

        public void AdvanceProgress()
        {
            _currentProgress = Mathf.Clamp(_currentProgress + 1, 0, 3);
            _musicInstance.setParameterByName(AudioEventNames.MusicProgressParam, _currentProgress);
        }

        public void SetProgress(int stage)
        {
            _currentProgress = Mathf.Clamp(stage, 0, 3);
            _musicInstance.setParameterByName(AudioEventNames.MusicProgressParam, _currentProgress);
        }

        private void OnDestroy()
        {
            if (_musicInstance.isValid())
            {
                _musicInstance.stop(STOP_MODE.IMMEDIATE);
                _musicInstance.release();
            }
        }

        public void StartBreakdown()
        {
            SetArea(MusicArea.Breakdown);
        }

        public void StopBreakdown()
        {
            HandleSceneMusic(SceneManager.GetActiveScene().name);
        }
    }
}
