using System;
using System.Collections.Generic;
using FMODUnity;
using Managers;
using UnityEngine;

namespace Audio
{
    [Serializable]
    public class SceneAudioReferences
    {
        public string sceneName;
        public EventReferenceWithName[] musicEvents;
        public EventReferenceWithName[] sfxEvents;
        public EventReferenceWithName[] ambianceEvents;
        public EventReferenceWithName[] voiceEvents;
    }
    [Serializable]
    public class EventReferenceWithName
    {
        [Tooltip("Must match a constant name in AudioEventNames.")]
        public string name;
        public EventReference eventReference;
    }
    public class FMODEvents : PersistentMonoSingleton<FMODEvents>
    {
        [SerializeField] private SceneAudioReferences[] sceneAudioReferences;

        private readonly Dictionary<string, EventReference> _eventReferences = new Dictionary<string, EventReference>();

        protected override void Awake()
        {
            base.Awake();
            // Add all event references to the dictionary
            InitializeEventReferences();
        }
        
        private void InitializeEventReferences()
        {
            foreach (var sceneAudioReference in sceneAudioReferences)
            {
                foreach (var ambianceEvent in sceneAudioReference.ambianceEvents)
                {
                    AddEventReference(ambianceEvent);
                }
                foreach (var sfxEvent in sceneAudioReference.sfxEvents)
                {
                    AddEventReference(sfxEvent);
                }
                foreach (var musicEvent in sceneAudioReference.musicEvents)
                {
                    AddEventReference(musicEvent);
                }
                foreach (var voiceEvent in sceneAudioReference.voiceEvents)
                {
                    AddEventReference(voiceEvent);
                }
            }
        }

        private void AddEventReference(EventReferenceWithName eventReferenceWithName)
        {
            if (_eventReferences.ContainsKey(eventReferenceWithName.name))
            {
                // Debug.LogWarning($"Duplicate event reference name '{eventReferenceWithName.name}' found. Overwriting previous entry.");
            }
            _eventReferences[eventReferenceWithName.name] = eventReferenceWithName.eventReference;
        }
        
        public EventReference GetEventReferenceByName(string eventName)
        {
            return _eventReferences[eventName];
        }
    }
}
