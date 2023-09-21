using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DocumentSlot : Slot
{
    public override void UpdateSlot(int id)
    {
        base.UpdateSlot(id);
        
        UpdateSlot(DocumentManager.instance.InInventory(id),
            DocumentManager.instance.documents.GetIcon(id));
    }
    
    public void UpdateSlot(bool active, Sprite sprite)
    {
        this.active = active;
        gameObject.SetActive(active);
        this.sprite = active ? sprite : null;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
       DocumentManager.instance.UpdateDocumentUI(ID);
       base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }
}
