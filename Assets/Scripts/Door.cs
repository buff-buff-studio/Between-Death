using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Refactor.Props
{
    public class Door : Interactable
    {
        private Animator animator;
        [SerializeField] private KeyData key;
        [SerializeField] private string openAnimationName = "Open";
        [SerializeField] private string closeAnimationName = "Close";

        public UnityEvent onOpen;
        public UnityEvent onClose;
        private string animation => state ? openAnimationName : closeAnimationName;
        
        private void Awake()
        {
            TryGetComponent(out animator);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!callOnEnable) return;

            animator.Play(animation);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        public override void Interact()
        {
            if(key != null && !InGameHUD.instance.HasKey(key)) return;
            base.Interact();
            InGameHUD.instance.UseKey(key);
            animator.Play(animation);
            switch (state)
            {
                case true:
                    onOpen.Invoke();
                    break;
                case false:
                    onClose.Invoke();
                    break;
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Door), true), CanEditMultipleObjects]
    public sealed class DoorEditor : InteractableEditor
    {
        private SerializedProperty _key;
        private SerializedProperty _openAnimationName;
        private SerializedProperty _closeAnimationName;
        private SerializedProperty _onOpen;
        private SerializedProperty _onClose;

        private List<string> animations = new List<string>();
        private int _openIndex = 0;
        private int _closeIndex = 0;

        protected override void OnEnable()
        {
            base.OnEnable();

            var door = (Door)this.target;

            _key = serializedObject.FindProperty("key");
            _openAnimationName = serializedObject.FindProperty("openAnimationName");
            _closeAnimationName = serializedObject.FindProperty("closeAnimationName");
            _onOpen = serializedObject.FindProperty("onOpen");
            _onClose = serializedObject.FindProperty("onClose");

            animations.Clear();
            var animationController = door.GetComponent<Animator>().runtimeAnimatorController;
            foreach (var anim in animationController.animationClips) animations.Add(anim.name);
            _openIndex = animations.Exists(x => x == _openAnimationName.stringValue)
                ? animations.IndexOf(_openAnimationName.stringValue)
                : 0;
            _closeIndex = animations.Exists(x => x == _closeAnimationName.stringValue)
                ? animations.IndexOf(_closeAnimationName.stringValue)
                : 0;
        }

        public override void OnInspectorGUI()
        {
            var target = (Interactable)this.target;

            serializedObject.Update();
            if (!target.TryGetComponent(out Animator _))
                EditorGUILayout.HelpBox("This component requires an Animator component", MessageType.Error);

            EditorGUILayout.Space(2f);
            InteractProperty();

            EditorGUILayout.Space(10f);
            var typeChange = ColliderProperty(target);

            EditorGUILayout.Space(10f);
            AnchorAxisProperty();

            EditorGUILayout.Space(5f);
            EditorGUILayout.PropertyField(_key);
            _openIndex = EditorGUILayout.Popup("Active Animation", _openIndex, animations.ToArray());
            _openAnimationName.stringValue = animations[_openIndex];
            _closeIndex = EditorGUILayout.Popup("Inactive Animation", _closeIndex, animations.ToArray());
            _closeAnimationName.stringValue = animations[_closeIndex];

            EditorGUILayout.Space(5f);
            EditorGUILayout.PropertyField(_callOnEnable);
            EditorGUILayout.PropertyField(_onInteract);
            EditorGUILayout.PropertyField(_onOpen);
            EditorGUILayout.PropertyField(_onClose);

            serializedObject.ApplyModifiedProperties();

            if (typeChange) ChangeColliderType();
            UpdateCollider();
        }
    }
#endif
}