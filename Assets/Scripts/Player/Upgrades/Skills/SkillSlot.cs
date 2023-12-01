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
            SkillManager.instance.skills.GetName(id), 0);
        button.interactable = !SkillManager.instance.IsEquipped(id);
    }
    
    public override void UpdateSlot(bool active, Sprite sprite, string text, float cooldown)
    {
        this.active = active;
        this.sprite = active ? sprite : null;
        this.text = active ? text : "";
    }
    
    public void OnClick()
    {
        if(active) SkillManager.instance.Select(ID);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        button ??= GetComponent<Button>();
    }    
#endif
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if(active) SkillManager.instance.UpdateInfo(ID);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        return;
    }
}
