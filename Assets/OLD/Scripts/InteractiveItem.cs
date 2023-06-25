using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InteractiveItem : InteractiveObject
{
    [SerializeField]
    private ItemData itemData;
    
    [Space]
    public UnityEvent<ItemData> onItemPickedUp;
    
    protected override void Interact(InputAction.CallbackContext callbackContext)
    {
        if(!_canInteract) return;
        
        base.Interact(callbackContext);
        
        if(GameManager.Instance.itemData == null)
        {
            GameManager.Instance.AddItemToInventory(itemData);
            GameManager.Instance.InteractIconVisibility(false,this);
            onItemPickedUp?.Invoke(itemData);
            Destroy(this.gameObject);
        }
    }
}
