using System.Collections;
using UnityEngine;

namespace Objects
{
    public class MiniGame3PimpleInteractable : MonoBehaviour
    {
        [SerializeField] private Animator pimpleAnimator;
        [SerializeField] private Collider2D pimpleCollider;
        [SerializeField] private string popTriggerName = "Pop";
        [SerializeField, Min(0f)] private float popAnimationDuration = 0.5f;

        private bool _hasPopped;

        public bool HasPopped => _hasPopped;

        private void Awake()
        {
            if (pimpleAnimator == null)
                pimpleAnimator = GetComponent<Animator>();

            if (pimpleCollider == null)
                pimpleCollider = GetComponent<Collider2D>();
        }

        public void Pop(MonoBehaviour coroutineRunner)
        {
            if (_hasPopped) return;

            _hasPopped = true;

            if (pimpleCollider != null)
                pimpleCollider.enabled = false;

            if (pimpleAnimator == null)
                return;

            pimpleAnimator.enabled = true;
            pimpleAnimator.SetTrigger(popTriggerName);

            if (coroutineRunner != null)
                coroutineRunner.StartCoroutine(DisableAnimatorAfterPop());
        }

        private IEnumerator DisableAnimatorAfterPop()
        {
            yield return new WaitForSeconds(popAnimationDuration);

            if (pimpleAnimator != null)
                pimpleAnimator.enabled = false;
        }
    }
}
