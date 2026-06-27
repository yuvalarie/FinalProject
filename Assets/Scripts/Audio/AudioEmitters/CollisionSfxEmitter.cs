using System;
using UnityEngine;

namespace Audio.AudioEmitters
{
    public class CollisionSfxEmitter : SfxEmitter
    {
        public enum CollisionSource { Self, Children }

        [Header("Collision")]
        [SerializeField] private CollisionSource collisionSource = CollisionSource.Self;

        private void Start()
        {
            if (collisionSource == CollisionSource.Children)
                SetupChildRelays();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collisionSource == CollisionSource.Self)
                PlayAudioOnce();
        }

        private void SetupChildRelays()
        {
            foreach (var col in GetComponentsInChildren<Collider2D>(true))
            {
                if (col.gameObject == gameObject) continue;
                if (col.gameObject.GetComponent<CollisionRelay>() == null)
                    col.gameObject.AddComponent<CollisionRelay>().OnCollision = PlayAudioOnce;
            }
        }
    }

    internal class CollisionRelay : MonoBehaviour
    {
        public Action OnCollision;

        private void OnCollisionEnter2D(Collision2D _) => OnCollision?.Invoke();
    }
}
