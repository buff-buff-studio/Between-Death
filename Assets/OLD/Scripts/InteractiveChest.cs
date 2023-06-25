using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InteractiveChest : InteractiveObject
{
    [Space]
    public DocumentData DocumentData;
    
    [Space]
    public UnityEvent onChestOpened;
    
    private bool _isOpen = false;

    protected override void Interact(InputAction.CallbackContext callbackContext)
    {
        if(!_canInteract || _isOpen || DocumentData == null) return;
        
        base.Interact(callbackContext);

        //GameManager.Instance._documentVariable.Value = DocumentData;
        
        GameManager.Instance.HUDSetVisible(false);
        onChestOpened?.Invoke();
        _isOpen = true;
        CancelInteract();
        SceneManager.LoadScene("Docs", LoadSceneMode.Additive);
    }
}
