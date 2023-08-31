using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillSlot : Slot
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
        
        UpdateSlot(SkillManager.instance.InInventory(id),
            SkillManager.instance.skills.GetIcon(id),
            SkillManager.instance.skills.GetName(id));
        button.interactable = !SkillManager.instance.IsEquipped(id);
    }
    
    public void OnClick()
    {
        SkillManager.instance.Select(ID);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        button ??= GetComponent<Button>();
    }    
#endif
    public override void OnPointerEnter(PointerEventData eventData)
    {
        SkillManager.instance.UpdateInfo(ID);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        return;
    }
}
