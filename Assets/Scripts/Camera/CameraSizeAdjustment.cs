using UnityEngine;

namespace Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class CameraSizeAdjustment : MonoBehaviour
    {
        [Tooltip("The target aspect ratio. 16:9 is 1.777...")]
        public float targetAspect = 16f / 9f;

        [SerializeField, Tooltip("Refresh the camera viewport if the build resolution changes after scene start.")]
        private bool updateWhenResolutionChanges = true;

        [SerializeField, Tooltip("Log camera viewport changes to help compare Editor and Build behavior.")]
        private bool logViewportChanges;

        private UnityEngine.Camera _camera;
        private int _lastScreenWidth;
        private int _lastScreenHeight;

        private void Awake()
        {
            _camera = GetComponent<UnityEngine.Camera>();
        }

        void Start()
        {
            ForceAspectRatio();
        }

        private void Update()
        {
            if (!updateWhenResolutionChanges)
            {
                return;
            }

            if (Screen.width == _lastScreenWidth && Screen.height == _lastScreenHeight)
            {
                return;
            }

            ForceAspectRatio();
        }

        private void ForceAspectRatio()
        {
            if (Screen.height <= 0 || Mathf.Approximately(targetAspect, 0f))
            {
                return;
            }

            if (_camera == null)
            {
                _camera = GetComponent<UnityEngine.Camera>();
            }

            _lastScreenWidth = Screen.width;
            _lastScreenHeight = Screen.height;

            // Determine the current screen proportion
            float windowAspect = (float)Screen.width / (float)Screen.height;

            // Compare it to your target (16:9)
            float scaleHeight = windowAspect / targetAspect;

            // If the screen is narrower than 16:9 (e.g., 16:10 or 4:3) -> Letterbox (Top/Bottom bars)
            if (scaleHeight < 1.0f)
            {
                Rect rect = _camera.rect;
                rect.width = 1.0f;
                rect.height = scaleHeight;
                rect.x = 0;
                rect.y = (1.0f - scaleHeight) / 2.0f;
                _camera.rect = rect;
            }
            // If the screen is wider than 16:9 (e.g., Ultra-wide) -> Pillarbox (Left/Right bars)
            else 
            {
                float scaleWidth = 1.0f / scaleHeight;
                Rect rect = _camera.rect;
                rect.width = scaleWidth;
                rect.height = 1.0f;
                rect.x = (1.0f - scaleWidth) / 2.0f;
                rect.y = 0;
                _camera.rect = rect;
            }

            if (logViewportChanges)
            {
                Debug.Log($"[CameraSizeAdjustment] {name}: screen={Screen.width}x{Screen.height}, targetAspect={targetAspect:F4}, cameraRect={_camera.rect}");
            }
        }
    }
}
