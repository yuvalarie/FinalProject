using UnityEngine;

namespace Objects {
    public class TidyObject : MonoBehaviour {
        [SerializeField, Tooltip("The exact spot where this object belongs.")]
        private Transform _designatedPlace;

        // A public property so the NPC can ask the object where it needs to go
        public Transform DesignatedPlace => _designatedPlace;

        // This draws a helpful blue line in the editor so you can see where objects belong!
        private void OnDrawGizmosSelected() {
            if (_designatedPlace != null) {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, _designatedPlace.position);
                Gizmos.DrawWireSphere(_designatedPlace.position, 0.3f);
            }
        }
    }
}