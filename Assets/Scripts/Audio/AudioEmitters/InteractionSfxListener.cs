using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Audio.AudioEmitters
{
    public class InteractionSfxListener : AudioEmitterBase
    {
        [SerializeField, AudioEventName] private string interactionEventName;
        
        private InputSystem_Actions _inputActions;
        
        private void Awake()
        {
            _inputActions = new InputSystem_Actions();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _inputActions.Game.Enable();
            _inputActions.Game.Interact.performed += OnInteraction;
            // _inputActions.Game.Interact.canceled += OnInteraction;
        }
        
        private void OnDisable()
        {
            _inputActions.Game.Interact.performed -= OnInteraction;
            // _inputActions.Game.Interact.canceled -= OnInteraction;
            _inputActions.Game.Disable();
        }
        
        private void OnInteraction(InputAction.CallbackContext context)
        {
            StartAudio();
        }
        

        public override void SetAudioEventName()
        {
            var field = typeof(AudioEventNames).GetField(interactionEventName, BindingFlags.Public | BindingFlags.Static);
            AudioEventName = (string)field.GetValue(null);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _inputActions?.Dispose();
        }
    }
}
