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

        [Header("Tool Detection Settings")]
        [Tooltip("Only used by tool stations. How far this tool can search while held.")]
        [Min(0f)] public float detectionRadius;
        [Tooltip("Only used by tool stations. Where this tool searches from when facing right.")]
        public Transform rightGrabSlot;
        [Tooltip("Only used by tool stations. Where this tool searches from when facing left.")]
        public Transform leftGrabSlot;
        
        [Space(10)]
        public bool isArea;
        [Tooltip("Only set this if isArea is true")]
        public AreaType areaType;
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.grey;
            if(rightGrabSlot != null) Gizmos.DrawWireSphere(rightGrabSlot.position, detectionRadius);
            Gizmos.color = Color.purple;
            if(leftGrabSlot != null) Gizmos.DrawWireSphere(leftGrabSlot.position, detectionRadius);
        }
    }
}
