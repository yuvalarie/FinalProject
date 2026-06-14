using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Managers;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Audio
{
    public class AudioManager : PersistentMonoSingleton<AudioManager>
    {
        [Header("Volumes")]
        [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
        private Bus _masterBus;
        [SerializeField, Range(0f, 1f)] private float musicVolume = 1f;
        private Bus _musicBus;
        [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;
        private Bus _sfxBus;
        [SerializeField, Range(0f, 1f)] private float ambianceVolume = 1f;
        private Bus _ambianceBus;
        
        private readonly List<EventInstance> _activeEvents = new List<EventInstance>();
        private readonly List<StudioEventEmitter> _activeEmitters = new List<StudioEventEmitter>();
        // private List<EventInstance> _ambianceEvents = new List<EventInstance>();


        private void Awake()
        {
            _masterBus = RuntimeManager.GetBus("bus:/");
            _musicBus = RuntimeManager.GetBus("bus:/Music");
            _sfxBus = RuntimeManager.GetBus("bus:/SFX");
            _ambianceBus = RuntimeManager.GetBus("bus:/Ambiance");
        }

        private void Update()
        { 
            // might want to change this to only update when the volume sliders are changed, but for now it should be fine
            _masterBus.setVolume(masterVolume);
            _musicBus.setVolume(musicVolume);
            _sfxBus.setVolume(sfxVolume);
            _ambianceBus.setVolume(ambianceVolume);
        }
        
        public void PlayOneShot(EventReference eventReference, Vector3 worldPosition, params (string name, float value)[] parameters)
        {
            //Debug.Log($"Playing one-shot event: {eventReference.Path} at position {worldPosition}");
            var eventInstance = RuntimeManager.CreateInstance(eventReference);
            foreach (var (name, value) in parameters)
            {
                eventInstance.setParameterByName(name, value);
            }
            eventInstance.set3DAttributes(worldPosition.To3DAttributes());
            eventInstance.start();
            eventInstance.release(); // Release the instance immediately since it's a one-shot
        }
        
        public EventInstance CreateInstance(EventReference eventReference, params(string paramName, float value)[] parameters)
        {
            var eventInstance = RuntimeManager.CreateInstance(eventReference);
            foreach (var (paramName, value) in parameters)
            {
                eventInstance.setParameterByName(paramName, value);
            }
            _activeEvents.Add(eventInstance);
            return eventInstance;
        }

        public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject,
            params (string paramName, float value)[] parameters)
        {
            var emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
            emitter.EventReference = eventReference;
            
            foreach (var (paramName, value) in parameters)
            {
                emitter.SetParameter(paramName, value);
            }
            _activeEmitters.Add(emitter);
            return emitter;
        }

        private void CleanUp()
        {
            // clean up any emitters that haven't finished playing
            foreach (var eventInstance in _activeEvents)
            {
                eventInstance.stop(STOP_MODE.IMMEDIATE);
                eventInstance.release();
            }

            foreach (var emitter in _activeEmitters)
            {
                emitter.Stop();
            }
            
        }

        private void OnDestroy()
        {
            CleanUp();
        }
    }
}
