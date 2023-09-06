using System;
using System.Collections;
using System.Collections.Generic;
using Refactor;
using Refactor.Interface;
using Refactor.Props;
using UnityEngine;

public class InteractibleManager : MonoBehaviour
{
    public static InteractibleManager instance;
    
    [SerializeField] private RectTransform _interactibleIcon;
    [SerializeField] private Interactible interactibleObject;
    [SerializeField] private IngameCanvas _ingameCanvas;
    public InspectDoc documentInspect;
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
        Debug.Log("Interact");
        interactibleObject.Interact();
        if(interactibleObject.oneInteraction) _interactibleIcon.gameObject.SetActive(false);
        interactibleObject = null;
        _distance = 0;
    }

    public void OpenDocument(DocumentData doc)
    {
        _ingameCanvas.OpenDocumentWindow();
        documentInspect._documentData = doc as DocumentText;
    }
    
    private void Update()
    {
        if (interactibleObject == null) return;
        
        _interactibleIcon.position = Camera.main.WorldToScreenPoint(interactibleObject.interactionPoint);
        if(IngameGameInput.InputInteract.trigger) OnInteract();
    }
}
