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
        button.onClick.AddListener(OnClick);
    }

    public override void UpdateSlot(int id)
    {
        base.UpdateSlot(id);
        
        UpdateSlot(true,
            PassiveManager.instance.passives.GetIcon(id),
            PassiveManager.instance.passives.GetName(id));
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
