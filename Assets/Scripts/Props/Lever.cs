using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Refactor.Props
{
    public class Lever : Interactible
    {
        [Space]
        public string activeAnimationName = "Active";
        public string inactiveAnimationName = "Inactive";

        public UnityEvent onOn;
        public UnityEvent onOff;
        
        protected override void OnEnable()
        {
            base.OnEnable();

            onInteract.AddListener(Toggle);

            if (!callOnEnable) return;
            
            if (state)
                onOn.Invoke();
            else
                onOff.Invoke();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            onInteract.RemoveListener(Toggle);
        }

        public void Toggle(bool state)
        {
            if (this.state)
                onOn.Invoke();
            else
                onOff.Invoke();
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(Lever), true), CanEditMultipleObjects]
    public sealed class LeverEditor : InteractibleEditor
    {
        private SerializedProperty _activeAnimationName;
        private SerializedProperty _inactiveAnimationName;
        
        private List<string> animations = new List<string>();
        private int _activeIndex = 0;
        private int _inactiveIndex = 0;

        protected override void OnEnable()
        {
            var target = (Lever) this.target;
            
            _activeAnimationName = serializedObject.FindProperty("activeAnimationName");
            _inactiveAnimationName = serializedObject.FindProperty("inactiveAnimationName");
            
            animations.Clear();
            var animationController = target.GetComponent<Animator>().runtimeAnimatorController;
            foreach (var anim in animationController.animationClips) animations.Add(anim.name);
            _activeIndex = animations.IndexOf(_activeAnimationName.stringValue);
            _inactiveIndex = animations.IndexOf(_inactiveAnimationName.stringValue);
            
            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            var target = (Interactible) this.target;
            
            serializedObject.Update();
            
            EditorGUILayout.Space(2f);
            InteractProperty();

            EditorGUILayout.Space(10f);
            var typeChange = ColliderProperty(target);

            EditorGUILayout.Space(10f);
            AnchorAxisProperty();
            
            EditorGUILayout.Space(5f);
            _activeIndex = EditorGUILayout.Popup("Active Animation", _activeIndex, animations.ToArray());
            _activeAnimationName.stringValue = animations[_activeIndex];
            _inactiveIndex = EditorGUILayout.Popup("Inactive Animation", _inactiveIndex, animations.ToArray());
            _inactiveAnimationName.stringValue = animations[_inactiveIndex];
            
            EditorGUILayout.PropertyField(_callOnEnable);
            EditorGUILayout.PropertyField(_onInteract);

            serializedObject.ApplyModifiedProperties();
            
            if (typeChange) ChangeColliderType();
            UpdateCollider();
        }
    }
#endif
}