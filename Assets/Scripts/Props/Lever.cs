using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Refactor.Props
{

    [RequireComponent(typeof(Animator))]
    public class Lever : Interactible
    {
        private Animator animator;
        [SerializeField] private string activeAnimationName = "Open";
        [SerializeField] private string inactiveAnimationName = "Open";

        public UnityEvent onOn;
        public UnityEvent onOff;

        private void Awake()
        {
            TryGetComponent(out animator);
        }

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
        private SerializedProperty _onOn;
        private SerializedProperty _onOff;
        
        private List<string> animations = new List<string>();
        private int _activeIndex = 0;
        private int _inactiveIndex = 0;

        protected override void OnEnable()
        {
            base.OnEnable();

            var lever = (Lever) this.target;

            _activeAnimationName = serializedObject.FindProperty("openAnimationName");
            _inactiveAnimationName = serializedObject.FindProperty("openAnimationName");
            _onOn = serializedObject.FindProperty("onOn");
            _onOff = serializedObject.FindProperty("onOff");

            animations.Clear();
            var animationController = lever.GetComponent<Animator>().runtimeAnimatorController;
            foreach (var anim in animationController.animationClips) animations.Add(anim.name);
            _activeIndex = animations.Exists(x => x == _activeAnimationName.stringValue) ? animations.IndexOf(_activeAnimationName.stringValue) : 0;
            _inactiveIndex = animations.Exists(x => x == _inactiveAnimationName.stringValue) ? animations.IndexOf(_inactiveAnimationName.stringValue) : 0;
        }

        public override void OnInspectorGUI()
        {
            var target = (Interactible) this.target;

            serializedObject.Update();
            if(!target.TryGetComponent(out Animator _))
                EditorGUILayout.HelpBox("This component requires an Animator component", MessageType.Error);

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

            EditorGUILayout.Space(5f);
            EditorGUILayout.PropertyField(_callOnEnable);
            EditorGUILayout.PropertyField(_onInteract);
            EditorGUILayout.PropertyField(_onOn);
            EditorGUILayout.PropertyField(_onOff);

            serializedObject.ApplyModifiedProperties();

            if (typeChange) ChangeColliderType();
            UpdateCollider();
        }
    }
#endif
}