using UnityEngine;
using UnityEngine.Serialization;

namespace Objects
{
    public class GrabbableObject : MonoBehaviour
    {
        public enum ObjectState { Start, Held, Placed }

        [Header("Current State")]
        public ObjectState currentState = ObjectState.Start;
        
        [Header("Placement Settings")]
        [Tooltip("The specific trigger collider where this object belongs.")]
        public Collider2D targetDropSpot;
    }
}