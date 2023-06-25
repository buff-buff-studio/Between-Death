using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(SphereCollider))]
public class InteractiveObject : MonoBehaviour
{
    [SerializeField]
    internal SphereCollider _interactCollider;
    [SerializeField] internal Transform _interactPoint;
    
    [SerializeField]
    private float _interactRadius = 1.5f;
    [SerializeField]
    private float _interactDistance = 1f;
    
    [SerializeField]
    private bool _useOtherCollider = false;
    
    private bool _isInteracting = false;
    public bool _canInteract = false;
    
    //private GameInputs input;
    protected internal float distance;

    private void Awake()
    {
        //input = new GameInputs();
        
        //input.Main.Interact.performed += Interact;
    }
    
    private void OnEnable()
    {
        //input.Enable();
    }

    private void OnDisable()
    {
        //input.Disable();
    }

    private void OnDestroy()
    {
        //input.Main.Interact.performed -= Interact;
    }

    private void Update()
    {
        if(_isInteracting) GameManager.Instance.InteractIconPos(_interactPoint.position, this);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!col.CompareTag("Player")) return;
        
        distance = Vector3.Distance(col.transform.position, transform.position);
        GameManager.Instance.InteractIconVisibility(true, this);
        _isInteracting = true;
    }

    private void OnTriggerStay(Collider col)
    {
        if (!col.CompareTag("Player")) return;
        
        distance = Vector3.Distance(col.transform.position, transform.position);

        if (distance < _interactDistance)
        {
            GameManager.Instance.InteractIconSize(75, this);
            if (!_canInteract) _canInteract = true;
        }
        else
        {
            GameManager.Instance.InteractIconSize(25, this);
            
            if (_canInteract) _canInteract = false;
        }
    }
    
    private void OnTriggerExit(Collider col)
    {
        if (!col.CompareTag("Player")) return;
        
        _isInteracting = false;
        GameManager.Instance.InteractIconVisibility(false,this);
    }

    protected virtual void Interact(InputAction.CallbackContext callbackContext)
    {
        if(!_canInteract) return;
        
        Debug.LogWarning("Interacting with " + gameObject.name);
    }
    
    public void SetInteractPoint()
    {
        var p = GetComponentsInChildren<Transform>().ToList().Find(x =>
            x.name.Equals("InteractIcon", StringComparison.InvariantCultureIgnoreCase));

        _interactPoint = p != null ? p : GameObject.Instantiate(Resources.Load<GameObject>("InteractIcon"), this.transform).transform;
        //_interactPoint = p != null ? p : InteractiveUtility.TrySafeInstantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Interact/InteractIcon.prefab"), this.transform).transform;
    }
    
    public void CancelInteract()
    {
        _interactCollider.enabled = false;
        _isInteracting = false;
        GameManager.Instance.InteractIconVisibility(false,this);
    }

    private void OnValidate()
    {
        _interactDistance = Mathf.Clamp(_interactDistance, 0, _interactRadius);
        
        _interactCollider ??= GetComponent<SphereCollider>();
        if (!_useOtherCollider)
        {
            if (_interactCollider == null) return;
            _interactCollider.radius = _interactRadius;
            _interactCollider.isTrigger = true;
        }
        else
        {
            _interactCollider.enabled = false;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(InteractiveObject), true), CanEditMultipleObjects]
public class InteractiveObjectEditor : Editor
{
    private InteractiveObject _target;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        _target = target as InteractiveObject;

        if (_target._interactPoint != null) return;
        
        if (GUILayout.Button("Set Interact Point"))
        {
            _target.SetInteractPoint();
        }
    }
}
#endif
