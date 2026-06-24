using System;
using Objects.Poster;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Pages
{
    public class PlayerControllerPage13 : PlayerControllerBase
    {
        [Header("Poster Settings")] 
        [SerializeField] private PosterDisplay poster1;
        [SerializeField] private Collider2D poster1Collider;
        [SerializeField] private GameObject rolledPoster1;
        [SerializeField] private GameObject poster1Location;
        
        [SerializeField] private PosterDisplay poster2;
        [SerializeField] private Collider2D poster2Collider;
        [SerializeField] private GameObject rolledPoster2;
        [SerializeField] private GameObject poster2Location;
        
        [SerializeField] private PosterDisplay poster3;
        [SerializeField] private Collider2D poster3Collider;
        [SerializeField] private GameObject rolledPoster3;
        [SerializeField] private GameObject poster3Location;
        
        private bool _atPosterCollider1;
        private bool _atPosterCollider2;
        private bool _atPosterCollider3;
        private bool _isPlaced1;
        private bool _isPlaced2;
        private bool _isPlaced3;

        protected override void Start()
        {
            base.Start();
            poster1.LoadPoster();
            poster1.gameObject.SetActive(false);
            poster2.LoadPoster();
            poster2.gameObject.SetActive(false);
            poster3.LoadPoster();
            poster3.gameObject.SetActive(false);
        }

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if (_atPosterCollider1)
            {
                _isPlaced1 = true;
                poster1.gameObject.transform.position = poster1Location.transform.position;
                poster1.gameObject.transform.parent = null;
                poster1.gameObject.SetActive(true);
                poster1Collider.gameObject.SetActive(false);
                Destroy(rolledPoster1);
            }
            
            if (_atPosterCollider2)
            {
                _isPlaced2 = true;
                poster2.gameObject.transform.position = poster2Location.transform.position;
                poster2.gameObject.transform.parent = null;
                poster2.gameObject.SetActive(true);
                poster2Collider.gameObject.SetActive(false);
                Destroy(rolledPoster2);
            }
            
            if (_atPosterCollider3)
            {
                _isPlaced3 = true;
                poster3.gameObject.transform.position = poster3Location.transform.position;
                poster3.gameObject.transform.parent = null;
                poster3.gameObject.SetActive(true);
                poster3Collider.gameObject.SetActive(false);
                Destroy(rolledPoster3);
            }
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            if (other == poster1Collider)
            {
                _atPosterCollider1 = true;
                rolledPoster1.SetActive(false);
                poster1.gameObject.SetActive(true);
            }
            if (other == poster2Collider)
            {
                _atPosterCollider2 = true;
                rolledPoster2.SetActive(false);
                poster2.gameObject.SetActive(true);
            }
            if (other == poster3Collider)
            {
                _atPosterCollider3 = true;
                rolledPoster3.SetActive(false);
                poster3.gameObject.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other == poster1Collider)
            {
                _atPosterCollider1 = false;
                rolledPoster1.SetActive(true);
                if (!_isPlaced1) poster1.gameObject.SetActive(false);
            }
            if (other == poster2Collider)
            {
                _atPosterCollider2 = false;
                rolledPoster2.SetActive(true);
                if (!_isPlaced2) poster2.gameObject.SetActive(false);
            }
            if (other == poster3Collider)
            {
                _atPosterCollider3 = false;
                rolledPoster3.SetActive(true);
                if (!_isPlaced3) poster3.gameObject.SetActive(false);
            }
        }
    }
}