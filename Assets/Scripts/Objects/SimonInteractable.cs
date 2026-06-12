using UnityEngine;

namespace Objects
{
    public enum ToolType { None, Purple, Orange, Green, Yellow }
    public enum AreaType { None, Top, BottomCenter, BottomLeft, BottomRight }

    public class SimonInteractable : MonoBehaviour
    {
        [Header("What is this object?")]
        public bool isToolStation;
        [Tooltip("Only set this if isToolStation is true")]
        public ToolType toolType;
        
        [Space(10)]
        public bool isArea;
        [Tooltip("Only set this if isArea is true")]
        public AreaType areaType;
    }
}