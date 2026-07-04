using UnityEngine;

namespace Camera
{
    [RequireComponent(typeof(RectTransform))]
    public class FixedAspectCanvasArea : MonoBehaviour
    {
        [SerializeField, Tooltip("The camera that defines the playable 16:9 area. If empty, the Canvas camera or Main Camera is used.")]
        private UnityEngine.Camera targetCamera;

        [SerializeField, Tooltip("Fallback aspect ratio used if no camera with a viewport is found.")]
        private float targetAspect = 16f / 9f;

        [SerializeField, Tooltip("Log changes when the UI area is resized. Useful for checking builds.")]
        private bool logChanges;

        private RectTransform _rectTransform;
        private Canvas _canvas;
        private Rect _lastAppliedRect;
        private int _lastScreenWidth;
        private int _lastScreenHeight;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();
        }

        private void OnEnable()
        {
            ApplyAspectArea(force: true);
        }

        private void LateUpdate()
        {
            ApplyAspectArea(force: false);
        }

        private void ApplyAspectArea(bool force)
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }

            Rect viewportRect = GetViewportRect();
            bool screenChanged = Screen.width != _lastScreenWidth || Screen.height != _lastScreenHeight;

            if (!force && !screenChanged && Approximately(_lastAppliedRect, viewportRect))
            {
                return;
            }

            _lastScreenWidth = Screen.width;
            _lastScreenHeight = Screen.height;
            _lastAppliedRect = viewportRect;

            _rectTransform.anchorMin = new Vector2(viewportRect.xMin, viewportRect.yMin);
            _rectTransform.anchorMax = new Vector2(viewportRect.xMax, viewportRect.yMax);
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;
            _rectTransform.localScale = Vector3.one;
            _rectTransform.localRotation = Quaternion.identity;

            if (logChanges)
            {
                Debug.Log($"[FixedAspectCanvasArea] {name}: screen={Screen.width}x{Screen.height}, anchors={viewportRect}");
            }
        }

        private Rect GetViewportRect()
        {
            UnityEngine.Camera cameraForArea = targetCamera;

            if (cameraForArea == null && _canvas != null)
            {
                cameraForArea = _canvas.worldCamera;
            }

            if (cameraForArea == null)
            {
                cameraForArea = UnityEngine.Camera.main;
            }

            if (cameraForArea != null)
            {
                return cameraForArea.rect;
            }

            return CalculateViewportRectFromScreen();
        }

        private Rect CalculateViewportRectFromScreen()
        {
            if (Screen.height <= 0 || Mathf.Approximately(targetAspect, 0f))
            {
                return new Rect(0f, 0f, 1f, 1f);
            }

            float windowAspect = (float)Screen.width / Screen.height;
            float scaleHeight = windowAspect / targetAspect;

            if (scaleHeight < 1f)
            {
                return new Rect(0f, (1f - scaleHeight) * 0.5f, 1f, scaleHeight);
            }

            float scaleWidth = 1f / scaleHeight;
            return new Rect((1f - scaleWidth) * 0.5f, 0f, scaleWidth, 1f);
        }

        private static bool Approximately(Rect first, Rect second)
        {
            return Mathf.Approximately(first.x, second.x)
                && Mathf.Approximately(first.y, second.y)
                && Mathf.Approximately(first.width, second.width)
                && Mathf.Approximately(first.height, second.height);
        }
    }
}
