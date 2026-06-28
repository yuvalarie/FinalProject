using FMOD.Studio;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Audio
{
    public class MusicManager : PersistentMonoSingleton<MusicManager>
    {
        [Header("Scene Names")]
        [Tooltip("When this scene loads, Opening music stops and Main Game music begins")]
        [SerializeField] private string mainGameSceneName;

        private EventInstance _openingInstance;
        private EventInstance _gameInstance;
        private bool _openingIsPlaying;
        private bool _mainGameStarted;

        private void Start()
        {
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
            if (sceneName == mainGameSceneName)
                TransitionToMainGame();
            else if (!_mainGameStarted)
                PlayOpeningMusic();
        }

        private void PlayOpeningMusic()
        {
            if (_openingIsPlaying) return;

            var eventRef = FMODEvents.Instance.GetEventReferenceByName(AudioEventNames.MusicOpening);
            _openingInstance = AudioManager.Instance.CreateInstance(eventRef);
            _openingInstance.start();
            _openingIsPlaying = true;
        }

        private void TransitionToMainGame()
        {
            if (_gameInstance.isValid()) return;

            if (_openingIsPlaying)
            {
                _openingInstance.stop(STOP_MODE.ALLOWFADEOUT);
                _openingInstance.release();
                _openingIsPlaying = false;
            }

            _mainGameStarted = true;
            var eventRef = FMODEvents.Instance.GetEventReferenceByName(AudioEventNames.MusicGame);
            _gameInstance = AudioManager.Instance.CreateInstance(eventRef);
            _gameInstance.start();
        }

        private void OnDestroy()
        {
            if (_openingInstance.isValid())
            {
                _openingInstance.stop(STOP_MODE.IMMEDIATE);
                _openingInstance.release();
            }

            if (_gameInstance.isValid())
            {
                _gameInstance.stop(STOP_MODE.IMMEDIATE);
                _gameInstance.release();
            }
        }
    }
}
