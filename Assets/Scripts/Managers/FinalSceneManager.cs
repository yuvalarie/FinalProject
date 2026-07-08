using System;
using System.Collections;
using Objects;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class FinalSceneManager : MonoBehaviour
    {
        [SerializeField] private float animationDuration;

        private void Start()
        {
            StartCoroutine(EndCoroutine());
        }

        private IEnumerator EndCoroutine()
        {
            yield return new WaitForSeconds(animationDuration);
            if (SceneManager.sceneCountInBuildSettings <= 0)
            {
                Debug.LogWarning("[DebugSceneNavigator] Cannot restart game because no scenes are configured in Build Settings.");
                yield break;
            }

            Debug.Log("[FinalSceneManager] Restarting game from build scene 0.");
            MechanicalCounter.ClearPersistedValues();
            SceneManager.LoadScene(0);
        }
    }
}