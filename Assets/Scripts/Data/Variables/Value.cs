using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Refactor.Data.Variables
{
    [Serializable]
    public struct Value<T>
    {
        [SerializeField]
        private T localValue;
        [SerializeField]
        private Variable<T> variable;

        public Variable currentVariable => variable;

        public Action onValueChange;

        public T value
        {
            get
            {
                return variable == null ? localValue : variable.Value;
            }
            set
            {
                if (variable == null)
                    localValue = value;
                else
                    variable.Value = value;

                onValueChange?.Invoke();
            }
        }

        public bool hasVariable => variable != null;

        public static implicit operator Value<T>(T value)
        {
            return new Value<T> { value = value };
        }

        public static implicit operator T(Value<T> value)
        {
            return value.value;
        }

        #region Operators
        public static Value<T> operator +(Value<T> a, Value<T> b) => _RunOperator(a, b.value, Expression.Add);
        public static Value<T> operator +(Value<T> a, T b) => _RunOperator(a, b, Expression.Add);

        public static Value<T> operator -(Value<T> a, Value<T> b) => _RunOperator(a, b.value, Expression.Subtract);
        public static Value<T> operator -(Value<T> a, T b) => _RunOperator(a, b, Expression.Subtract);

        public static Value<T> operator *(Value<T> a, Value<T> b) => _RunOperator(a, b.value, Expression.Multiply);
        public static Value<T> operator *(Value<T> a, T b) => _RunOperator(a, b, Expression.Multiply);

        public static Value<T> operator /(Value<T> a, Value<T> b) => _RunOperator(a, b.value, Expression.Divide);
        public static Value<T> operator /(Value<T> a, T b) => _RunOperator(a, b, Expression.Divide);
        
        public static Value<T> operator &(Value<T> a, Value<T> b) => _RunOperator(a, b.value, Expression.And);
        public static Value<T> operator &(Value<T> a, T b) => _RunOperator(a, b, Expression.And);

        public static Value<T> operator |(Value<T> a, Value<T> b) => _RunOperator(a, b.value, Expression.OrElse);
        public static Value<T> operator |(Value<T> a, T b) => _RunOperator(a, b, Expression.OrElse);

        public static Value<T> operator ^(Value<T> a, Value<T> b) => _RunOperator(a, b.value, Expression.ExclusiveOr);
        public static Value<T> operator ^(Value<T> a, T b) => _RunOperator(a, b, Expression.ExclusiveOr);
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Value<T> _Copy(T newValue)
        {
            Value<T> copy = new Value<T> { variable = this.variable };
            copy.value = newValue;

            return copy;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Value<T> _RunOperator(Value<T> a, T b, Func<Expression, Expression, BinaryExpression> exp)
        {
            ParameterExpression paramA = Expression.Parameter(typeof(T), "a");
            ParameterExpression paramB = Expression.Parameter(typeof(T), "b");

            BinaryExpression body = exp(paramA, paramB);

            Func<T, T, T> func = Expression.Lambda<Func<T, T, T>>(body, paramA, paramB).Compile();
            return a._Copy(func(a.value, b));
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(Value<>), true)]
    public class ValueDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var textDimensions = GUI.skin.label.CalcSize(new GUIContent("text"));

            Rect prePosition = position;
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            GUIStyle style = EditorStyles.boldLabel;
            Color preColor = style.normal.textColor;
            style.normal.textColor = Color.yellow;
            Vector2 v2 = style.CalcSize(label);
            prePosition.position += new Vector2(v2.x, 0);
            EditorGUI.PrefixLabel(prePosition, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("(Value)"), style);
            style.normal.textColor = preColor;

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            SerializedProperty propVariable = property.FindPropertyRelative("variable");

            if (propVariable.objectReferenceValue == null)
            {
                float offset = position.width - 20;
                var rectVal = new Rect(position.x, position.y, offset - 4, position.height);
                var rectVar = new Rect(position.x + offset, position.y, 20, position.height - 2);

                EditorGUI.PropertyField(rectVal, property.FindPropertyRelative("localValue"), GUIContent.none, true);
                EditorGUI.PropertyField(rectVar, propVariable, GUIContent.none, true);
            }
            else
            {
                EditorGUI.PropertyField(position, propVariable, GUIContent.none, true);
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }
    }
#endif
}