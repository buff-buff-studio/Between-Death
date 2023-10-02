using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Refactor.Props
{
    public class Item : Interactable
    {
        [SerializeField] private KeyData key;
        [SerializeField] private bool isSceneItem = false;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!callOnEnable) return;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        public override void Interact()
        {
            if(!isSceneItem)
            {
                InGameHUD.instance.OpenItem(key);
                base.Interact();
                Destroy(gameObject);
            }
            else if(key != null && InGameHUD.instance.HasKey(key))
            {
                InGameHUD.instance.UseKey(key);
                base.Interact();
                SetEnabled(false);
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Item), true), CanEditMultipleObjects]
    public sealed class ItemEditor : InteractableEditor
    {
        private SerializedProperty _key;
        private SerializedProperty _isSceneItem;
        private int _type = 0;

        protected override void OnEnable()
        {
            base.OnEnable();

            var door = (Item)this.target;

            _key = serializedObject.FindProperty("key");
            _isSceneItem = serializedObject.FindProperty("isSceneItem");
            _type = _isSceneItem.boolValue ? 0 : 1;
        }

        public override void OnInspectorGUI()
        {
            var target = (Interactable)this.target;

            serializedObject.Update();

            EditorGUILayout.Space(2f);
            InteractProperty();

            EditorGUILayout.Space(10f);
            var typeChange = ColliderProperty(target);

            EditorGUILayout.Space(10f);
            AnchorAxisProperty();

            EditorGUILayout.Space(5f);
            EditorGUILayout.PropertyField(_key);
            _type = EditorGUILayout.Popup("Interactable Type", _type, new string[] { "Scene Item", "Inventory Item" });
            _isSceneItem.boolValue = _type == 0;

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