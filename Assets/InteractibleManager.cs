using System;
using System.Collections;
using System.Collections.Generic;
using Refactor.Props;
using UnityEngine;

public class InteractibleManager : MonoBehaviour
{
    public static InteractibleManager instance;
    
    [SerializeField] private RectTransform _interactibleIcon;
    public Interactible interactibleObject;
    private bool _canInteract;
    private float _distance;

    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(this);
        
        _interactibleIcon.gameObject.SetActive(false);
    }

    public void OnInteractibleEnter(Interactible interactible, float distance, bool canInteract)
    {
        if (interactible != interactibleObject && distance <= _distance) return;
        
        _distance = distance;
        _canInteract = canInteract;
        interactibleObject = interactible;
        
        _interactibleIcon.sizeDelta = _canInteract ? new Vector2(100, 100) : new Vector2(50, 50);
        _interactibleIcon.gameObject.SetActive(true);
    }
    
    public void OnInteractibleExit(Interactible interactible)
    {
        if (interactible != interactibleObject) return;
        
        interactibleObject = null;
        _interactibleIcon.gameObject.SetActive(false);
    }

    private void OnInteract()
    {
        if (interactibleObject == null || _canInteract == false) return;
        interactibleObject.Interact();
        interactibleObject = null;
        _distance = 0;
    }
    
    private void Update()
    {
        if (interactibleObject == null) return;
        
        _interactibleIcon.position = Camera.main.WorldToScreenPoint(interactibleObject.transform.position);
    }
}
