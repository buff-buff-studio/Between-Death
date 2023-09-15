using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Refactor.Props
{
    [RequireComponent(typeof(Animator))]
    public class Chest : Interactible
    {
        private Animator animator;
        [SerializeField] private ChestItem _item;
        [SerializeField] private string openAnimationName = "Open";

        private void Awake()
        {
            TryGetComponent(out animator);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!callOnEnable) return;

            if (state)
            {
                animator.Play(openAnimationName);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        public override void Interact()
        {
            base.Interact();
            if(state)
            {
                _item.OpenItem();
                animator.Play(openAnimationName);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            oneInteraction = true;
        }
#endif
    }
    
    public class ChestItem : ScriptableObject{ public virtual void OpenItem(){} }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(Chest), true), CanEditMultipleObjects]
    public sealed class ChestEditor : InteractibleEditor
    {
        private SerializedProperty _item;
        private SerializedProperty _openAnimationName;
        
        private List<string> animations = new List<string>();
        private int _openIndex = 0;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            var chest = (Chest) this.target;
            
            _item = serializedObject.FindProperty("_item");
            _openAnimationName = serializedObject.FindProperty("openAnimationName");
            
            animations.Clear();
            var animationController = chest.GetComponent<Animator>().runtimeAnimatorController;
            foreach (var anim in animationController.animationClips) animations.Add(anim.name);
            _openIndex = animations.Exists(x => x == _openAnimationName.stringValue) ? animations.IndexOf(_openAnimationName.stringValue) : 0;
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
            EditorGUILayout.PropertyField(_item);
            _openIndex = EditorGUILayout.Popup("Active Animation", _openIndex, animations.ToArray());
            _openAnimationName.stringValue = animations[_openIndex];
            
            EditorGUILayout.Space(5f);
            EditorGUILayout.PropertyField(_callOnEnable);
            EditorGUILayout.PropertyField(_onInteract);

            serializedObject.ApplyModifiedProperties();
            
            if (typeChange) ChangeColliderType();
            UpdateCollider();
        }
    }
#endif
}