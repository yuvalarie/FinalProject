using UnityEngine;
using Objects; // Make sure this matches the namespace of your TidyObject

namespace Npc {
    [RequireComponent(typeof(Rigidbody2D))]
    public class CleaningNpcController : MonoBehaviour {
        [SerializeField, Tooltip("The main destination the NPC is ultimately trying to reach.")]
        private Transform _finalTarget;

        [SerializeField, Tooltip("How fast the NPC walks.")]
        private float _walkSpeed = 3f;

        [SerializeField, Tooltip("How close the NPC needs to be to drop the item.")]
        private float _dropDistance = 0.2f;

        private Rigidbody2D _rb;
        private TidyObject _heldItem;
        private bool _isCarryingItem;

        private void Awake() {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate() {
            // Determine our current goal: Are we taking an item to its place, or going to our final target?
            Transform currentDestination = _isCarryingItem ? _heldItem.DesignatedPlace : _finalTarget;

            if (currentDestination != null) {
                // Calculate direction and move using the 2D Rigidbody
                float distanceX = currentDestination.position.x - _rb.position.x;
                Vector2 direction = new Vector2(distanceX, 0).normalized;
                _rb.linearVelocity = new Vector2(direction.x * _walkSpeed, _rb.linearVelocity.y);

                // Check if we are carrying something AND we have reached its drop spot
                if (_isCarryingItem)
                {
                    float absoluteDistanceX = Mathf.Abs(distanceX);
                    if (absoluteDistanceX <= _dropDistance) {
                        DropItem();
                    }
                }
            } else {
                _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            // If we bump into an object, check if it's a TidyObject AND we aren't already holding something
            if (!_isCarryingItem && collision.gameObject.TryGetComponent(out TidyObject item)) {
                PickUpItem(item);
            }
        }

        private void PickUpItem(TidyObject item) {
            _heldItem = item;
            _isCarryingItem = true;

            // Turn off physics on the item so it doesn't bump the NPC while being carried
            Collider2D itemCollider = item.GetComponent<Collider2D>();
            if (itemCollider != null) itemCollider.enabled = false;

            // Parent it to the NPC so it moves with them visually
            item.transform.SetParent(this.transform);
            item.transform.localPosition = new Vector3(0, 0.5f, 0); // Hold it slightly above the NPC's head
        }

        private void DropItem() {
            // Unparent the item and snap it exactly to its designated place
            _heldItem.transform.SetParent(null);
            _heldItem.transform.position = _heldItem.DesignatedPlace.position;

            // Turn physics back on so it exists in the world again
            Collider2D itemCollider = _heldItem.GetComponent<Collider2D>();
            //if (itemCollider != null) itemCollider.enabled = true;

            // Clear our state so the NPC resumes walking to the _finalTarget
            _heldItem = null;
            _isCarryingItem = false;
        }
    }
}