using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    public class MiniGame3PimpleAnimator : MonoBehaviour
    {
        private static readonly int Pop = Animator.StringToHash("Pop");
        [SerializeField] private Animator pimple1Animator;
        [SerializeField] private Animator pimple2Animator;
        [SerializeField] private Animator pimple3Animator;
        [SerializeField] private Animator pimple4Animator;
        [SerializeField] private Animator pimple5Animator;

        [SerializeField] private float wait1;
        [SerializeField] private float wait2;
        [SerializeField] private float wait3;
        [SerializeField] private float wait4;
        [SerializeField] private float wait5;

        private void Start()
        {
            StartCoroutine(PimpleCoroutine());
        }

        private IEnumerator PimpleCoroutine()
        {
            yield return new WaitForSeconds(wait1);
            pimple1Animator.SetTrigger(Pop);
            yield return new WaitForSeconds(wait2);
            pimple2Animator.SetTrigger(Pop);
            yield return new WaitForSeconds(wait3);
            pimple3Animator.SetTrigger(Pop);
            yield return new WaitForSeconds(wait4);
            pimple4Animator.SetTrigger(Pop);
            yield return new WaitForSeconds(wait5);
            pimple5Animator.SetTrigger(Pop);
        }
    }
}