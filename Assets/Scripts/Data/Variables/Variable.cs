using System;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Refactor.Data.Variables
{
    /// <summary>
    /// Base generic variable class
    /// </summary>
    public class Variable : ScriptableObject
    {
        public UnityEvent onChanged;
        public virtual string ToJson()  => throw new NotImplementedException();
        public virtual void LoadFromJson(string json)  => throw new NotImplementedException();

        public virtual void Reset() => throw new NotImplementedException();
        public virtual void ForceUpdate() => throw new NotImplementedException();
    }

    /// <summary>
    /// Base typed variable class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Variable<T> : Variable
    {
        /// <summary>
        /// Holds variable raw value
        /// (Used for JSON serialization)
        /// </summary>
        [Serializable]
        public class ValueHolder
        {
            public T value;

            public ValueHolder(T value)
            {
                this.value = value;
            }
        }

        public T Value
        {
            get => _value;
            set
            {
                if(_value == null && value == null)
                    return;

                if(_value != null && _value.Equals(value))
                    return;

                _value = value;

                ForceUpdate();
            }
        }
        
        public UnityEvent<T> onValueChanged;

        [SerializeField]
        private T _value;

        public T defaultValue;
        
        public bool shouldReset = true;

        public override string ToString()
        {
            return _value.ToString();
        }

        public override void Reset() 
        {
            if(shouldReset)
                Value = defaultValue;
        }

        public override void ForceUpdate()
        {
            onValueChanged?.Invoke(_value);
            onChanged?.Invoke();
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(new ValueHolder(_value));
        }

        public override void LoadFromJson(string json)
        {
            ValueHolder holder = new ValueHolder(default(T));
            JsonUtility.FromJsonOverwrite(json, holder);
            _value = holder.value;
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(Variable), true)]
    public class VariableEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Reset"))
                (target as Variable)!.Reset();

            if (GUILayout.Button("Force Update"))
                (target as Variable)!.ForceUpdate();
        }
    }


    [CustomPropertyDrawer(typeof(Variable), true)]
    public class VariableDrawer : PropertyDrawer
    {   
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            float halfWidth = position.width / 2;

            if(property.objectReferenceValue == null)
            {
                EditorGUI.PropertyField(position, property, GUIContent.none, true);
            }
            else
            {
                SerializedObject obj = new SerializedObject(property.objectReferenceValue);

                var rectVar = new Rect(position.x, position.y, halfWidth - 4, position.height);
                var rectVal = new Rect(position.x + halfWidth, position.y, halfWidth, position.height - 2);

                EditorGUI.PropertyField(rectVal, obj.FindProperty("_value"), GUIContent.none);
                obj.ApplyModifiedProperties();

                EditorGUI.PropertyField(rectVar, property, GUIContent.none, true);        
            }

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
    #endif

}