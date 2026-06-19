using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Audio.Editor
{
    [CustomPropertyDrawer(typeof(AudioEventNameAttribute))]
    public class AudioEventNameDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var fields = typeof(AudioEventNames)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
                .ToArray();

            var names = fields.Select(f => f.Name).ToArray();

            int currentIndex = Array.IndexOf(names, property.stringValue);
            if (currentIndex < 0) currentIndex = 0;

            EditorGUI.BeginProperty(position, label, property);
            int newIndex = EditorGUI.Popup(position, label.text, currentIndex, names);
            property.stringValue = names[newIndex];
            EditorGUI.EndProperty();
        }
    }
}
