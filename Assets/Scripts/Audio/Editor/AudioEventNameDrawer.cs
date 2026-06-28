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
        private static string[] _cachedNames;

        private static string[] GetNames()
        {
            if (_cachedNames != null) return _cachedNames;
            _cachedNames = typeof(AudioEventNames)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
                .Select(f => f.Name)
                .ToArray();
            return _cachedNames;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var names = GetNames();

            int currentIndex = Array.IndexOf(names, property.stringValue);
            if (currentIndex < 0) currentIndex = 0;

            const float buttonWidth = 24f;
            var popupRect = new Rect(position.x, position.y, position.width - buttonWidth - 2f, position.height);
            var buttonRect = new Rect(position.x + position.width - buttonWidth, position.y, buttonWidth, position.height);

            EditorGUI.BeginProperty(position, label, property);

            int newIndex = EditorGUI.Popup(popupRect, label.text, currentIndex, names);
            if (newIndex != currentIndex)
                property.stringValue = names[newIndex];

            if (GUI.Button(buttonRect, "⌕"))
            {
                PopupWindow.Show(buttonRect, new AudioEventSearchPopup(
                    names,
                    property.serializedObject,
                    property.propertyPath));
            }

            EditorGUI.EndProperty();
        }
    }

    internal class AudioEventSearchPopup : PopupWindowContent
    {
        private readonly string[] _allNames;
        private readonly SerializedObject _serializedObject;
        private readonly string _propertyPath;

        private string _search = "";
        private string[] _filtered;
        private Vector2 _scroll;

        public AudioEventSearchPopup(string[] names, SerializedObject serializedObject, string propertyPath)
        {
            _allNames = names;
            _serializedObject = serializedObject;
            _propertyPath = propertyPath;
            _filtered = names;
        }

        public override Vector2 GetWindowSize() => new Vector2(260f, 320f);

        public override void OnOpen()
        {
            EditorGUIUtility.editingTextField = true;
        }

        public override void OnGUI(Rect rect)
        {
            EditorGUI.BeginChangeCheck();
            GUI.SetNextControlName("SearchField");
            _search = EditorGUILayout.TextField(_search, EditorStyles.toolbarSearchField);
            if (EditorGUI.EndChangeCheck())
            {
                _filtered = string.IsNullOrEmpty(_search)
                    ? _allNames
                    : _allNames.Where(n => n.IndexOf(_search, StringComparison.OrdinalIgnoreCase) >= 0).ToArray();
                _scroll = Vector2.zero;
            }

            GUI.FocusControl("SearchField");

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            foreach (var name in _filtered)
            {
                if (GUILayout.Button(name, EditorStyles.label))
                {
                    var prop = _serializedObject.FindProperty(_propertyPath);
                    prop.stringValue = name;
                    _serializedObject.ApplyModifiedProperties();
                    editorWindow.Close();
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }
}
