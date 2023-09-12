using System;
using System.Collections;
using System.Collections.Generic;
using Refactor;
using Refactor.Interface;
using Refactor.Props;
using UnityEngine;
using UnityEngine.Serialization;

public class InGameHUD : MonoBehaviour
{
    public static InGameHUD instance;

    [Space]
    [Header("Menu References")]
    [SerializeField] private InGameMenu menu;
    
    [Space]
    [Header("Pop-Up References")]
    [SerializeField] private RectTransform popUpParent;
    
    [Space]
    [Header("Interaction References")]
    [SerializeField] private RectTransform interactibleIcon;
    [SerializeField] private Interactible interactibleObject;

    //Interaction References
    private bool _canInteract;
    private float _distance;

    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(this);
        
        interactibleIcon.gameObject.SetActive(false);
    }
    
    private void Update()
    {
        if (interactibleObject == null) return;
        
        interactibleIcon.position = Camera.main.WorldToScreenPoint(interactibleObject.interactionPoint);
        if(IngameGameInput.InputInteract.trigger) OnInteract();
    }

    public void OnInteractibleEnter(Interactible interactible, float distance, bool canInteract)
    {
        if (interactible != interactibleObject && distance <= _distance) return;
        
        _distance = distance;
        _canInteract = canInteract;
        interactibleObject = interactible;
        
        interactibleIcon.sizeDelta = _canInteract ? new Vector2(100, 100) : new Vector2(50, 50);
        interactibleIcon.gameObject.SetActive(true);
    }
    
    public void OnInteractibleExit(Interactible interactible)
    {
        if (interactible != interactibleObject) return;
        
        interactibleObject = null;
        interactibleIcon.gameObject.SetActive(false);
    }

    private void OnInteract()
    {
        if (interactibleObject == null || _canInteract == false) return;
        Debug.Log("Interact");
        interactibleObject.Interact();
        if(interactibleObject.oneInteraction) interactibleIcon.gameObject.SetActive(false);
        interactibleObject = null;
        _distance = 0;
    }

    public void OpenDocument(DocumentData doc)
    {
        //documentInspect._documentData = doc as DocumentText;
    }
}
