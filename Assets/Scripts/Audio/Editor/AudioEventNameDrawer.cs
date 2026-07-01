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
        private int _selectedIndex;
        private const float RowHeight = 18f;

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
            HandleKeyboardInput();

            EditorGUI.BeginChangeCheck();
            GUI.SetNextControlName("SearchField");
            _search = EditorGUILayout.TextField(_search, EditorStyles.toolbarSearchField);
            if (EditorGUI.EndChangeCheck())
            {
                _filtered = string.IsNullOrEmpty(_search)
                    ? _allNames
                    : _allNames.Where(n => n.IndexOf(_search, StringComparison.OrdinalIgnoreCase) >= 0).ToArray();
                _selectedIndex = 0;
                _scroll = Vector2.zero;
            }

            GUI.FocusControl("SearchField");

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            for (int i = 0; i < _filtered.Length; i++)
            {
                var rowRect = GUILayoutUtility.GetRect(1f, RowHeight, GUILayout.ExpandWidth(true));

                if (Event.current.type == EventType.MouseMove && rowRect.Contains(Event.current.mousePosition))
                {
                    _selectedIndex = i;
                    editorWindow.Repaint();
                }

                bool isSelected = i == _selectedIndex;
                if (Event.current.type == EventType.Repaint)
                {
                    var style = isSelected ? EditorStyles.selectionRect : GUIStyle.none;
                    style.Draw(rowRect, GUIContent.none, false, false, false, false);
                }

                if (GUI.Button(rowRect, _filtered[i], EditorStyles.label))
                {
                    Commit(_filtered[i]);
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private void HandleKeyboardInput()
        {
            var e = Event.current;
            if (e.type != EventType.KeyDown) return;

            switch (e.keyCode)
            {
                case KeyCode.DownArrow:
                    _selectedIndex = Mathf.Min(_selectedIndex + 1, _filtered.Length - 1);
                    ScrollToSelected();
                    e.Use();
                    break;
                case KeyCode.UpArrow:
                    _selectedIndex = Mathf.Max(_selectedIndex - 1, 0);
                    ScrollToSelected();
                    e.Use();
                    break;
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    if (_selectedIndex >= 0 && _selectedIndex < _filtered.Length)
                        Commit(_filtered[_selectedIndex]);
                    e.Use();
                    break;
                case KeyCode.Escape:
                    editorWindow.Close();
                    e.Use();
                    break;
            }
        }

        private void ScrollToSelected()
        {
            float rowTop = _selectedIndex * RowHeight;
            float rowBottom = rowTop + RowHeight;
            if (rowTop < _scroll.y)
                _scroll.y = rowTop;
            else if (rowBottom > _scroll.y + GetWindowSize().y - RowHeight * 2f)
                _scroll.y = rowBottom - (GetWindowSize().y - RowHeight * 2f);
        }

        private void Commit(string name)
        {
            var prop = _serializedObject.FindProperty(_propertyPath);
            prop.stringValue = name;
            _serializedObject.ApplyModifiedProperties();
            editorWindow.Close();
        }
    }
}
