using System;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor; 
#endif
namespace Refactor.Misc
{
    [Serializable]
    public class ResourceSlot<T> where T : Object
    {
        public string path;
        [SerializeField, HideInInspector]
        private T _buffer;
        
        /// <summary>
        /// Returns the actual resource reference
        /// </summary>
        public T value
        {
            get
            {
                if (_buffer == null && !string.IsNullOrEmpty(path))
                    _buffer = Resources.Load<T>(path);
                return _buffer;
            }
        }

        public static implicit operator ResourceSlot<T>(string path)
        {
            return new ResourceSlot<T>() { path = path };
        }

        public static implicit operator T(ResourceSlot<T> slot) => slot.value;
        public static implicit operator Object(ResourceSlot<T> slot) => slot.value;
    }
    
    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ResourceSlot<>), true)]
    public class ResourceSlotPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            GUIStyle style = EditorStyles.boldLabel;
            Color preColor = style.normal.textColor;
            style.normal.textColor = Color.magenta;
            var text = label.text;

            int textSplitPos = text.LastIndexOf('/');
            if (textSplitPos > -1)
                text = text.Substring(textSplitPos + 1);
            
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(text), style);
            style.normal.textColor = preColor;
            
            SerializedProperty pathProperty = property.FindPropertyRelative("path");
            SerializedProperty bufferProperty = property.FindPropertyRelative("_buffer");
            
            string path = pathProperty.stringValue;
            Type bufferType = bufferProperty.GetUnderlyingType();
            
            pathProperty.stringValue = _DoField(path, bufferType, position);

            EditorGUI.EndProperty();
        }
        
        private static string _DoField(string path, Type type, Rect rect)
        {
            Object oldObject = string.IsNullOrEmpty(path) ? null : Resources.Load(path, type);
            Object newObject = EditorGUI.ObjectField(rect, oldObject, type, false);
            
            string newPath = AssetDatabase.GetAssetPath(newObject);
                
            if (string.IsNullOrEmpty(newPath)) 
                return newPath;

            return newPath.Contains("Resources") ? newPath.Split("Resources", 2)[1][1..].Split("." , 2)[0] : path;
        }
    }
    #endif
}
