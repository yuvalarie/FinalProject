using UnityEngine;

namespace Managers
{
    public class PersistentMonoSingleton<T> : MonoSingleton<T> where T : MonoBehaviour
    {
        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
