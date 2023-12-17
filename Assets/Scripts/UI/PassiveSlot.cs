using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PassiveSlot : Slot
{
    [Space][Header("Click")]
    [SerializeField] private Button button;

    protected override void Start()
    {
        base.Start();

        if(button == null) return;
        button.onClick.AddListener(OnClick);
    }

    public override void UpdateSlot(int id)
    {
        base.UpdateSlot(id);
        UpdateSlot(PassiveManager.instance.InInventory(id),
            PassiveManager.instance.passives.GetIcon(id),
            PassiveManager.instance.passives.GetName(id), 0);

        if(button == null) return;
        button.interactable = !PassiveManager.instance.IsEquipped(id);
    }
    
    public void OnClick()
    {
        PassiveManager.instance.Select(ID);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        button ??= GetComponent<Button>();
    }    
#endif
    public override void OnPointerEnter(PointerEventData eventData)
    {
        PassiveManager.instance.UpdateInfo(ID);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        return;
    }
}
