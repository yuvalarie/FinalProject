using UnityEngine;

// namespace GameManagers
// {
namespace Managers
{
    /// <summary>
    /// A generic Singleton class for MonoBehaviours.
    /// Example usage: public class GameManager : MonoSingleton<GameManager>
    /// </summary>
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static bool _isQuitting = false;

        public static T Instance
        {
            get
            {
                if (_isQuitting) return null;
                if (_instance != null)
                    return _instance;

                _instance = FindFirstObjectByType<T>();
                if (_instance == null)
                {
                    var singletonObject = new GameObject(typeof(T).Name);
                    _instance = singletonObject.AddComponent<T>();
                    // all it did was if we created a new instance, then it won't destroy on load. if you want a singleton that goes between scenes, use PersistentMonoSingleton instead 
                    // DontDestroyOnLoad(singletonObject); // Don't destroy the object when loading a new scene
                }

                return _instance;
            }
        }

        private void OnApplicationQuit()
        {
            _isQuitting = true;
        }

        // Ensure no other instances can be created by having the constructor as protected
        protected MonoSingleton()
        {
        }
    }
}
// }