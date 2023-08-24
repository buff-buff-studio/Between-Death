using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Refactor.Props;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
using Unity.VisualScripting;
#endif

namespace Refactor.Props
{
    public enum ColliderType : byte
    {
        Sphere,
        Box,
        Prism
    }
    public enum AnchorAxis : byte
    {
        AxisNegative,
        AxisCenter,
        AxisPositive
    }
    
    public class Interactible : MonoBehaviour
    {
        //Interaction
        [SerializeField] [NotNull] private Transform interactionPointOffset;
        [SerializeField] private float time = 0f;

        //Collider
        [SerializeField] private float distanceToInteract = 1f;

        [Space(5f)] [Header("Callbacks")]
        public UnityEvent onInteract;
        public Vector3 interactionPoint => interactionPointOffset != null ? interactionPointOffset.position : transform.position;
        
        //Editor Utilities
        [SerializeField] private ColliderType colliderType;
        [SerializeField] private Vector3 vectorSize;
        [SerializeField] private AnchorAxis _anchorX;
        [SerializeField] private AnchorAxis _anchorY;
        [SerializeField] private AnchorAxis _anchorZ;
        public float radius { get => vectorSize.x; set => vectorSize = new Vector3(value, value, value); }
        public float size { get => vectorSize.x; set => vectorSize = new Vector3(value, value, value); }
        public float averageSize => (vectorSize.x + vectorSize.y + vectorSize.z) / 3f;
        public float interactSize => distanceToInteract / averageSize;
        public Vector3 worldSize => transform.lossyScale;

        protected virtual void Start() { }

        protected virtual void OnEnable() { }

        protected virtual void OnDisable() { }

        protected void OnTriggerEnter(Collider col)
        {
            if (!col.CompareTag("Player")) return;
            
            var distance = distanceToInteract >= Vector3.Distance(transform.position, col.transform.position);
            
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            
        }
#endif
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Interactible), true), CanEditMultipleObjects]
public class InteractibleEditor : Editor
{
    private SerializedProperty _interactionPointOffset;
    private SerializedProperty _time;
    private SerializedProperty _distanceToInteract;
    private SerializedProperty _vectorSize;
    private SerializedProperty _colliderType;
    private SerializedProperty _anchorX;
    private SerializedProperty _anchorY;
    private SerializedProperty _anchorZ;
    private SerializedProperty _onInteract;
    
    private Vector3 _offset;
    private List<string> animations = new List<string>();
    private int anim;
    
    private Texture2D _interactionPointIcon;
    
    //Foldout Groups
    private bool interact = true;
    private bool collider = true;
    private bool axis = false;

    private void OnEnable()
    {
        var target = (Interactible) this.target;
        
        _interactionPointOffset = serializedObject.FindProperty("interactionPointOffset");
        _time = serializedObject.FindProperty("time");
        _distanceToInteract = serializedObject.FindProperty("distanceToInteract");
        _vectorSize = serializedObject.FindProperty("vectorSize");
        _colliderType = serializedObject.FindProperty("colliderType");
        _anchorX = serializedObject.FindProperty("_anchorX");
        _anchorY = serializedObject.FindProperty("_anchorY");
        _anchorZ = serializedObject.FindProperty("_anchorZ");
        _onInteract = serializedObject.FindProperty("onInteract");
        
        if (target.TryGetComponent(out BoxCollider col))
        {
            var size = new Vector3(_vectorSize.vector3Value.x / target.worldSize.x,
                _vectorSize.vector3Value.y / target.worldSize.y,
                _vectorSize.vector3Value.z / target.worldSize.z);
            
            _offset = new Vector3(
                col.center.x - _anchorX.enumValueIndex switch
                {
                    2 => size.x/2f,
                    1 => 0f,
                    _ => -size.x/2f
                },
                col.center.y - _anchorY.enumValueIndex switch
                {
                    2 => size.y/2f,
                    1 => 0f,
                    _ => -size.y/2f
                },
                col.center.z - _anchorZ.enumValueIndex switch
                {
                    2 => size.z/2f,
                    1 => 0f,
                    _ => -size.z/2f
                });
        }
        else if (target.TryGetComponent(out SphereCollider sphere)) _offset = sphere.center;
        
        animations.Clear();
        var animationController = target.GetComponent<Animator>().runtimeAnimatorController;
        foreach (var anim in animationController.animationClips) animations.Add(anim.name);

        _interactionPointIcon = Resources.Load<Texture2D>("Editor/InteractIcon");
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
        anim = EditorGUILayout.Popup("Animation", anim, animations.ToArray());
        
        EditorGUILayout.PropertyField(_onInteract);

        serializedObject.ApplyModifiedProperties();
        
        if (typeChange) ChangeColliderType();
        UpdateCollider();
    }

    private void InteractProperty()
    {
        interact = EditorGUILayout.BeginFoldoutHeaderGroup(interact,
            new GUIContent("Interaction", _interactionPointIcon, "Interaction Settings"));
        if (interact)
        {
            EditorGUILayout.Space(2f);
            EditorGUILayout.PropertyField(_interactionPointOffset, new GUIContent("Interaction Point"));
            EditorGUILayout.PropertyField(_time);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }
    private bool ColliderProperty(Interactible target)
    {
        collider = EditorGUILayout.BeginFoldoutHeaderGroup(collider, new GUIContent("Collider ☼", "Collider Settings"));
        var typeChange = false;
        if (collider)
        {
            EditorGUILayout.Space(2f);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_colliderType);
            typeChange = EditorGUI.EndChangeCheck();

            //Collider Offset
            _offset = EditorGUILayout.Vector3Field("Offset", _offset);

            //Collider Size
            switch ((ColliderType)_colliderType.enumValueIndex)
            {
                case ColliderType.Sphere:
                    target.radius = Mathf.Clamp(target.radius, 0f, 10f);
                    target.radius = EditorGUILayout.Slider("Radius", target.radius, 0f, 10f);
                    break;
                case ColliderType.Box:
                    target.size = Mathf.Clamp(target.size, 0f, 10f);
                    target.size = EditorGUILayout.Slider("Size", target.size, 0f, 10f);
                    break;
                case ColliderType.Prism:
                    EditorGUILayout.PropertyField(_vectorSize, new GUIContent("Size"));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //Interact Distance
            var interactLimit = _colliderType.enumValueIndex == 2 ? target.averageSize : target.radius;
            _distanceToInteract.floatValue = Mathf.Clamp(_distanceToInteract.floatValue, 0f, interactLimit);
            EditorGUILayout.Slider(_distanceToInteract, 0f, interactLimit);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
        return typeChange;
    }
    private void AnchorAxisProperty()
    {
        axis = EditorGUILayout.BeginFoldoutHeaderGroup(axis, new GUIContent("Anchor Preset", EditorUtilities.Center, "Anchor Axis Presets"));
        if (axis)
        {
            EditorGUI.indentLevel++;

            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight + 10f);
            EditorGUI.BeginProperty(rect, new GUIContent("Anchor X"), _anchorX);
            EditorGUI.PrefixLabel(rect, new GUIContent("Anchor X"));

            rect.x += EditorGUIUtility.labelWidth;
            rect.width -= EditorGUIUtility.labelWidth;
            rect.width = Mathf.Max(25f, rect.width / 5f);

            _anchorX.enumValueIndex = (int)EditorAnchorAxis(_anchorX, rect,
                new Texture2D[3] { EditorUtilities.NegX, EditorUtilities.Center, EditorUtilities.PosX });
            EditorGUI.EndProperty();

            rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight + 10f);
            EditorGUI.BeginProperty(rect, new GUIContent("Anchor Y"), _anchorY);
            EditorGUI.PrefixLabel(rect, new GUIContent("Anchor Y"));

            rect.x += EditorGUIUtility.labelWidth;
            rect.width -= EditorGUIUtility.labelWidth;
            rect.width = Mathf.Max(25f, rect.width / 5f);

            _anchorY.enumValueIndex = (int)EditorAnchorAxis(_anchorY, rect,
                new Texture2D[3] { EditorUtilities.NegY, EditorUtilities.Center, EditorUtilities.PosY });
            EditorGUI.EndProperty();

            rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight + 10f);
            EditorGUI.BeginProperty(rect, new GUIContent("Anchor Z"), _anchorY);
            EditorGUI.PrefixLabel(rect, new GUIContent("Anchor Z"));

            rect.x += EditorGUIUtility.labelWidth;
            rect.width -= EditorGUIUtility.labelWidth;
            rect.width = Mathf.Max(25f, rect.width / 5f);

            _anchorZ.enumValueIndex = (int)EditorAnchorAxis(_anchorZ, rect,
                new Texture2D[3] { EditorUtilities.NegZ, EditorUtilities.Center, EditorUtilities.PosZ });
            EditorGUI.EndProperty();
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private AnchorAxis EditorAnchorAxis(SerializedProperty anchor, Rect rect, Texture2D[] icons = null)
    {
        icons ??= new Texture2D[3]{EditorUtilities.Center, EditorUtilities.Center, EditorUtilities.Center};
        
        var axis = (AnchorAxis)anchor.enumValueIndex;
        axis = EditorUtilities.EditorToggle(rect, axis == AnchorAxis.AxisNegative,
            new GUIContent(icons[0]), EditorUtilities.btnLeft) ? AnchorAxis.AxisNegative : axis;
        rect.x += rect.width;
        axis = EditorUtilities.EditorToggle(rect, axis == AnchorAxis.AxisCenter,
            new GUIContent(icons[1]), EditorUtilities.btnMid) ? AnchorAxis.AxisCenter : axis;
        rect.x += rect.width;
        axis = EditorUtilities.EditorToggle(rect, axis == AnchorAxis.AxisPositive,
            new GUIContent(icons[2]), EditorUtilities.btnRight) ? AnchorAxis.AxisPositive : axis;
        
        return axis;
    }

    private void UpdateCollider()
    {
        var target = (Interactible) this.target;
        
        if ((ColliderType)_colliderType.enumValueIndex == ColliderType.Sphere)
        {
            var col = target.GetComponents<SphereCollider>();

            col[0].radius = (target.radius/target.worldSize.x)/2;
            col[1].radius = (_distanceToInteract.floatValue/target.worldSize.x)/2;
            
            col[0].center = _offset;
            col[1].center = _offset;
            
            col[0].isTrigger = true;
            col[1].isTrigger = true;
            
            _anchorX.enumValueIndex = 1;
            _anchorY.enumValueIndex = 1;
            _anchorZ.enumValueIndex = 1;
        }
        else
        {
            var col = target.GetComponents<BoxCollider>();
            var size = new Vector3(_vectorSize.vector3Value.x / target.worldSize.x,
                _vectorSize.vector3Value.y / target.worldSize.y,
                _vectorSize.vector3Value.z / target.worldSize.z);
            
            col[0].size = size;
            col[1].size = size * target.interactSize;
            
            col[0].center = GetAnchorCenter(size);
            col[1].center = GetAnchorCenter(size * target.interactSize);
            
            col[0].isTrigger = true;
            col[1].isTrigger = true;
        }
    }

    private Vector3 GetAnchorCenter(Vector3 size)
    {
        var anchorCenter = new Vector3(
            _offset.x + _anchorX.enumValueIndex switch
            {
                2 => size.x / 2f,
                1 => 0f,
                _ => -size.x / 2f
            },
            _offset.y + _anchorY.enumValueIndex switch
            {
                2 => size.y / 2f,
                1 => 0f,
                _ => -size.y / 2f
            },
            _offset.z + _anchorZ.enumValueIndex switch
            {
                2 => size.z / 2f,
                1 => 0f,
                _ => -size.z / 2f
            });
        return anchorCenter;
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
#endif