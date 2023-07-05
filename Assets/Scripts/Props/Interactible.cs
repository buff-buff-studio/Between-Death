using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Refactor.Props;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Refactor.Props
{
    public enum ColliderType : byte
    {
        Sphere,
        Box,
        Prism
    }
    public class Interactible : MonoBehaviour
    {
        [Header("Interaction")]
        [SerializeField] [NotNull] private Transform interactionPointOffset = null;
        [SerializeField] private float time = 0f;

        [Space(5f)] [Header("Collider")]
        [SerializeField] private float distanceToInteract = 1f;
        [SerializeField] private ColliderType colliderType;
        [SerializeField] private Vector3 vectorSize;

        [Space(5f)] [Header("Callbacks")]
        public UnityEvent onInteract;
        public Vector3 interactionPoint => interactionPointOffset != null ? interactionPointOffset.position : transform.position;
        
        public float radius { get => vectorSize.x; set => vectorSize = new Vector3(value, value, value); }
        public float size { get => vectorSize.x; set => vectorSize = new Vector3(value, value, value); }
        public float averageSize => (vectorSize.x + vectorSize.y + vectorSize.z) / 3f;
        public float interactSize => distanceToInteract / averageSize;

        protected virtual void Start() { }

        protected virtual void OnEnable() { }

        protected virtual void OnDisable() { }

#if UNITY_EDITOR
        private void OnValidate()
        {
            distanceToInteract = Mathf.Clamp(distanceToInteract, 0f, radius);
            /*
            if(Application.isPlaying) return;

            if (!editOnCollider && !copyCollider)
            {
                var sp = TryGetComponent(out SphereCollider sphere);
                var bx = TryGetComponent(out BoxCollider box);

                if (true)
                {
                    if (sp) if (sphere.isTrigger) StartCoroutine(DestroyCollider(sphere));

                    if (!bx) _collider = gameObject.AddComponent<BoxCollider>();

                    var boxCollider = (BoxCollider)_collider;
                    
                    boxCollider.center = offset;
                    boxCollider.size = size;
                }
                else
                {
                    if (bx) if (box.isTrigger) StartCoroutine(DestroyCollider(box));

                    if (!sp) _collider = gameObject.AddComponent<SphereCollider>();

                    var sphereCollider = (SphereCollider)_collider;
                    radius = size.x;
                    sphereCollider.center = offset;
                    sphereCollider.radius = radius;
                }

                _collider.isTrigger = true;
                _collider.enabled = true;
            }
            else
            {
                switch (colliderType)
                {
                    case ColliderType.Box when TryGetComponent(out BoxCollider box):
                        size = box.size;
                        offset = box.center;
                        break;
                    case ColliderType.Sphere when TryGetComponent(out SphereCollider sphere):
                        radius = sphere.radius;
                        offset = sphere.center;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                copyCollider = editOnCollider;
            }
            */
        }
        
        private IEnumerator DestroyCollider(Collider collider)
        {
            yield return new WaitForEndOfFrame();
            DestroyImmediate(collider);
        }
#endif
    }
}

[CustomEditor(typeof(Interactible), true), CanEditMultipleObjects]
public class InteractibleEditor : Editor
{
    private SerializedProperty _interactionPointOffset;
    private SerializedProperty _time;
    private SerializedProperty _distanceToInteract;
    private SerializedProperty _vectorSize;
    private SerializedProperty _colliderType;
    private SerializedProperty _onInteract;
    
    private Vector3 _offset;
    private List<string> animations = new List<string>();
    private int anim;

    private void OnEnable()
    {
        var target = (Interactible) this.target;
        
        _interactionPointOffset = serializedObject.FindProperty("interactionPointOffset");
        _time = serializedObject.FindProperty("time");
        _distanceToInteract = serializedObject.FindProperty("distanceToInteract");
        _vectorSize = serializedObject.FindProperty("vectorSize");
        _colliderType = serializedObject.FindProperty("colliderType");
        _onInteract = serializedObject.FindProperty("onInteract");
        
        if (target.TryGetComponent(out BoxCollider col)) _offset = col.center;
        else if (target.TryGetComponent(out SphereCollider sphere)) _offset = sphere.center;
        
        animations.Clear();
        var animationController = target.GetComponent<Animator>().runtimeAnimatorController;
        foreach (var anim in animationController.animationClips) animations.Add(anim.name);
    }

    public override void OnInspectorGUI()
    {
        var target = (Interactible) this.target;
        
        serializedObject.Update();

        EditorGUILayout.PropertyField(_interactionPointOffset);
        EditorGUILayout.PropertyField(_time);

        EditorGUILayout.Space(10f);
        EditorGUILayout.LabelField("Collider", EditorStyles.largeLabel);
        
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(_colliderType);
        var typeChange = EditorGUI.EndChangeCheck();
        _offset = EditorGUILayout.Vector3Field("Offset", _offset);
        switch ((ColliderType)_colliderType.enumValueIndex)
        {
            case ColliderType.Sphere:
                target.radius = Mathf.Clamp(target.radius, 0f, float.MaxValue);
                target.radius = EditorGUILayout.FloatField("Radius", target.radius);
                break;
            case ColliderType.Box:
                target.size = Mathf.Clamp(target.size, 0f, float.MaxValue);
                target.size = EditorGUILayout.FloatField("Size", target.size);
                break;
            case ColliderType.Prism:
                EditorGUILayout.PropertyField(_vectorSize, new GUIContent("Size"));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var interactLimit = _colliderType.enumValueIndex == 2 ? target.averageSize : target.radius;
        _distanceToInteract.floatValue = Mathf.Clamp(_distanceToInteract.floatValue, 0f, interactLimit);
        EditorGUILayout.Slider(_distanceToInteract, 0f,interactLimit);
        
        EditorGUILayout.Space(20f);
        anim = EditorGUILayout.Popup("Animation", anim, animations.ToArray());
        
        EditorGUILayout.PropertyField(_onInteract);

        serializedObject.ApplyModifiedProperties();
        
        if (typeChange) ChangeColliderType();
        UpdateCollider();
    }

    private void UpdateCollider()
    {
        var target = (Interactible) this.target;
        
        if ((ColliderType)_colliderType.enumValueIndex == ColliderType.Sphere)
        {
            var col = target.GetComponents<SphereCollider>();
            
            col[0].radius = target.radius;
            col[1].radius = _distanceToInteract.floatValue;
            
            col[0].center = _offset;
            col[1].center = _offset;
            
            col[0].isTrigger = true;
            col[1].isTrigger = true;
        }
        else
        {
            var col = target.GetComponents<BoxCollider>();
            
            col[0].size = _vectorSize.vector3Value;
            col[1].size = _vectorSize.vector3Value * target.interactSize;
            
            col[0].center = _offset;
            col[1].center = _offset;
            
            col[0].isTrigger = true;
            col[1].isTrigger = true;
        }
    }

    private void ChangeColliderType()
    {
        var target = (Interactible) this.target;
        
        foreach (var collider in target.GetComponents(typeof(Collider))) DestroyImmediate(collider);

        if ((ColliderType)_colliderType.enumValueIndex == ColliderType.Sphere)
        {
            target.AddComponent<SphereCollider>();
            target.AddComponent<SphereCollider>();
        }
        else
        {
            target.AddComponent<BoxCollider>();
            target.AddComponent<BoxCollider>();
        }
    }
}