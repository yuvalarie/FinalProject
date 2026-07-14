using Objects;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace DebugTools
{
    public sealed class DebugSceneNavigator : MonoBehaviour
    {
        private static DebugSceneNavigator instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            if (instance != null)
            {
                return;
            }

            var navigatorObject = new GameObject(nameof(DebugSceneNavigator));
            instance = navigatorObject.AddComponent<DebugSceneNavigator>();
            DontDestroyOnLoad(navigatorObject);
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;

            if (DebugSceneHotkeys.QuitGamePressed(keyboard))
            {
                QuitGame();
                return;
            }

            if (DebugSceneHotkeys.TryReadSceneSlot(keyboard, out int sceneSlot))
            {
                LoadSceneSlot(sceneSlot);
                return;
            }

            if (DebugSceneHotkeys.RestartScenePressed(keyboard))
            {
                RestartCurrentScene();
                return;
            }

            if (DebugSceneHotkeys.NextScenePressed(keyboard))
            {
                LoadNextScene();
                return;
            }

            if (DebugSceneHotkeys.RestartGamePressed(keyboard))
            {
                RestartGame();
            }
        }

        public void QuitGame()
        {
            Debug.Log("[DebugSceneNavigator] Quitting game.");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void LoadSceneSlot(int sceneSlot)
        {
            if (!DebugSceneCatalog.TryGetScene(sceneSlot, out DebugSceneEntry entry))
            {
                Debug.LogWarning($"[DebugSceneNavigator] No debug scene slot exists for {sceneSlot}.");
                return;
            }

            if (entry.IsReserved)
            {
                Debug.LogWarning($"[DebugSceneNavigator] Scene slot {sceneSlot} is reserved for '{entry.DisplayName}'. Add the scene name to DebugSceneCatalog when it exists.");
                return;
            }

            LoadSceneByName(entry.SceneName, $"slot {sceneSlot}: {entry.DisplayName}");
        }

        public void RestartCurrentScene()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.buildIndex >= 0)
            {
                Debug.Log($"[DebugSceneNavigator] Restarting current scene: {activeScene.name}.");
                LoadScene(activeScene.buildIndex);
                return;
            }

            LoadSceneByName(activeScene.name, $"current scene: {activeScene.name}");
        }

        public void LoadNextScene()
        {
            int sceneCount = SceneManager.sceneCountInBuildSettings;
            if (sceneCount <= 0)
            {
                Debug.LogWarning("[DebugSceneNavigator] No scenes are configured in Build Settings.");
                return;
            }

            int currentBuildIndex = SceneManager.GetActiveScene().buildIndex;
            int nextBuildIndex = currentBuildIndex < 0 ? 0 : (currentBuildIndex + 1) % sceneCount;

            Debug.Log($"[DebugSceneNavigator] Loading next build scene: index {nextBuildIndex}.");
            LoadScene(nextBuildIndex);
        }

        public void RestartGame()
        {
            if (SceneManager.sceneCountInBuildSettings <= 0)
            {
                Debug.LogWarning("[DebugSceneNavigator] Cannot restart game because no scenes are configured in Build Settings.");
                return;
            }

            Debug.Log("[DebugSceneNavigator] Restarting game from build scene 0.");
            MechanicalCounter.ClearPersistedValues();
            LoadScene(0);
        }

        private static void LoadSceneByName(string sceneName, string label)
        {
            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogWarning($"[DebugSceneNavigator] Cannot load {label}. Scene '{sceneName}' is not in the enabled Build Settings scene list.");
                return;
            }

            Debug.Log($"[DebugSceneNavigator] Loading {label}.");
            LoadScene(sceneName);
        }

        private static void LoadScene(int buildIndex)
        {
            if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.ForceLoadScene(buildIndex);
                return;
            }

            SceneManager.LoadScene(buildIndex);
        }

        private static void LoadScene(string sceneName)
        {
            if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.ForceLoadScene(sceneName);
                return;
            }

            SceneManager.LoadScene(sceneName);
        }
    }
}
