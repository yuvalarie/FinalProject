using UnityEngine;

namespace Managers
{
    public class PersistentMonoSingleton<T> : MonoSingleton<T> where T : MonoBehaviour
    {
        protected virtual void Awake()
        {
            if (Instance != null && Instance != this as T)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }
    }
}
