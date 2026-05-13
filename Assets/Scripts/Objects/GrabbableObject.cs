using UnityEngine;

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
        
        [Header("Visual Settings")]
        [SerializeField] private GameObject startSprite;
        [SerializeField] private GameObject heldSprite;
        [SerializeField] private GameObject placedSprite;
        
        private void Start()
        {
            SwitchState();
        }

        public void SwitchState()
        {
            if (startSprite != null) startSprite.SetActive(false);
            if (heldSprite != null) heldSprite.SetActive(false);
            if (placedSprite != null) placedSprite.SetActive(false);

            switch (currentState)
            {
                case ObjectState.Start:
                    if (startSprite != null) startSprite.SetActive(true);
                    break;
                case ObjectState.Held:
                    if (heldSprite != null)
                    {
                        heldSprite.SetActive(true);
                        var spriteRenderer = heldSprite.GetComponent<SpriteRenderer>();
                        if (spriteRenderer != null) spriteRenderer.sortingOrder = 10;
                    }
                    break;
                case ObjectState.Placed:
                    if (placedSprite != null) placedSprite.SetActive(true);
                    break;
            }
        }

        public void CenterChildren()
        {
            if (startSprite != null) startSprite.transform.localPosition = Vector3.zero;
            if (heldSprite != null) heldSprite.transform.localPosition = Vector3.zero;
            if (placedSprite != null) placedSprite.transform.localPosition = Vector3.zero;
        }
    }
}