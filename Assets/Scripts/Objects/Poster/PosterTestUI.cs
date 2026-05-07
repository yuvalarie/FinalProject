using UnityEngine;
using UnityEngine.SceneManagement;

namespace Objects.Poster
{
    public class PosterTestUI : MonoBehaviour
    {
        [SerializeField] private PosterManager posterManager;
        [SerializeField] private string displaySceneName = "DisplayScene";

        // Linked to the "Save" Button
        public void OnSaveButtonPressed()
        {
            Debug.Log("<color=yellow>UI:</color> Save Button Clicked.");
            posterManager.SavePoster();
        }

        // Linked to the "Load Display" Button
        public void OnLoadDisplayButtonPressed()
        {
            Debug.Log("<color=yellow>UI:</color> Load Display Scene Button Clicked.");
        
            // Safety check: ensure the scene name is set
            if (!string.IsNullOrEmpty(displaySceneName))
            {
                SceneManager.LoadScene(displaySceneName);
            }
            else
            {
                Debug.LogError("UI: Display Scene Name is empty in the inspector!");
            }
        }
    }
}
