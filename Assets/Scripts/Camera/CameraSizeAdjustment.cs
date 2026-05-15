using UnityEngine;

namespace Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class CameraSizeAdjustment : MonoBehaviour
    {
        [Tooltip("The target aspect ratio. 16:9 is 1.777...")]
        public float targetAspect = 16f / 9f;

        void Start()
        {
            ForceAspectRatio();
        }

        // Optional: If you allow players to resize the window while playing, 
        // you can change 'Start' to 'Update'. Otherwise, Start is best for performance.
        private void ForceAspectRatio()
        {
            // Determine the current screen proportion
            float windowAspect = (float)Screen.width / (float)Screen.height;

            // Compare it to your target (16:9)
            float scaleHeight = windowAspect / targetAspect;

            UnityEngine.Camera cam = GetComponent<UnityEngine.Camera>();

            // If the screen is narrower than 16:9 (e.g., 16:10 or 4:3) -> Letterbox (Top/Bottom bars)
            if (scaleHeight < 1.0f)
            {
                Rect rect = cam.rect;
                rect.width = 1.0f;
                rect.height = scaleHeight;
                rect.x = 0;
                rect.y = (1.0f - scaleHeight) / 2.0f;
                cam.rect = rect;
            }
            // If the screen is wider than 16:9 (e.g., Ultra-wide) -> Pillarbox (Left/Right bars)
            else 
            {
                float scaleWidth = 1.0f / scaleHeight;
                Rect rect = cam.rect;
                rect.width = scaleWidth;
                rect.height = 1.0f;
                rect.x = (1.0f - scaleWidth) / 2.0f;
                rect.y = 0;
                cam.rect = rect;
            }
        }
    }
}