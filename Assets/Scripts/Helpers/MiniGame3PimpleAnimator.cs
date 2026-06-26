using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    public class MiniGame3PimpleAnimator : MonoBehaviour
    {
        [SerializeField] private Animator pimple1Animator;
        [SerializeField] private Animator pimple2Animator;
        [SerializeField] private Animator pimple3Animator;
        [SerializeField] private Animator pimple4Animator;
        [SerializeField] private Animator pimple5Animator;

        private void Start()
        {
            StartCoroutine(PimpleCoroutine());
        }

        private IEnumerator PimpleCoroutine()
        {
            yield return new WaitForSeconds(3f);
            pimple1Animator.SetTrigger("Pop");
            yield return new WaitForSeconds(10f);
            pimple2Animator.SetTrigger("Pop");
            yield return new WaitForSeconds(5f);
            pimple3Animator.SetTrigger("Pop");
            yield return new WaitForSeconds(7f);
            pimple4Animator.SetTrigger("Pop");
            yield return new WaitForSeconds(2f);
            pimple5Animator.SetTrigger("Pop");
        }
    }
}