using FMODUnity;
using UnityEngine;

namespace Objects
{
    public class MiniGame3Animator : MonoBehaviour
    {
        [SerializeField] private GameObject phone1;
        [SerializeField] private GameObject phone2;
        [SerializeField] private GameObject hand;

        public void SwitchPhones()
        {
            phone2.SetActive(true);
            hand.SetActive(true);
            phone1.SetActive(false);
        }


    }
}