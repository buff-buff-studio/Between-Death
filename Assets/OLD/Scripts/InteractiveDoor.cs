using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InteractiveDoor : InteractiveObject
{
    [Space]
    [SerializeField] [Tooltip("If is null, the door don't need a key to be unlocked")]
    private ItemData _keyItem;
    
    [SerializeField]
    private bool _canClose = false;
    [SerializeField]
    private bool _invokeOnCloseInStart = false;
    
    [Space]
    [SerializeField]
    private UnityEvent<bool> onDoorUnlocked, onDoorOpened, onDoorClosed;

    private bool _isLocked = true;
    private bool _isOpen = false;
    
    private void Start()
    {
        if(_invokeOnCloseInStart) onDoorClosed?.Invoke(false);
    }
    
    protected override void Interact(InputAction.CallbackContext callbackContext)
    {
        if(!_canInteract) return;
        
        base.Interact(callbackContext);
        
        if(_keyItem != null && _isLocked)
        {
            if (GameManager.Instance.itemData == _keyItem) UnlockDoor();
            else Debug.Log("Door locked");
        }
        else
        {
            if(!_isOpen) OpenDoor();
            else if(_canClose) CloseDoor();
        }
    }
    
    private void UnlockDoor()
    {
        GameManager.Instance.RemoveItemFromInventory();
        onDoorUnlocked?.Invoke(true);
        OpenDoor();

        Debug.Log("Door unlocked");
    }
    
    private void OpenDoor()
    {
        if(!_canClose) CancelInteract();
        _isLocked = false;
        _isOpen = true;
        onDoorOpened?.Invoke(_isOpen);
    }
    
    private void CloseDoor()
    {
        _isOpen = false;
        onDoorClosed?.Invoke(_isOpen);
    }
}
