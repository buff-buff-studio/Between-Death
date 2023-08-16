using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillItem : MonoBehaviour
{
    [SerializeField] private bool isEquipSlot = false;
    
    [SerializeField] private uint id;
    [SerializeField] private Image icon;
    [SerializeField] private Button button;
    [SerializeField] private TMPro.TextMeshProUGUI name;
    [SerializeField] private uint slotID;
    
    public Sprite sprite
    {
        get => icon.sprite;
        set => icon.sprite = value;
    }
    
    public string text
    {
        get => name.text;
        set => name.text = value;
    }

    public void Start()
    {
        UpdateSkill(id);
        button.onClick.AddListener(OnClick);
    }

    public void UpdateSkill(uint id)
    {
        this.id = id;
        gameObject.SetActive(SkillManager.instance.InInventory(id));
        icon.sprite = SkillManager.instance.skills.GetIcon((int)id);
        button.interactable = !SkillManager.instance.IsEquipped(id);

        if (!isEquipSlot) 
            name.text = SkillManager.instance.skills.GetName((int)id);
    }
    
    public void OnClick()
    {
        if (isEquipSlot)
            SkillManager.instance.Equip(id, slotID);
        else
            SkillManager.instance.Select(id);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        icon ??= transform.GetChild(0).GetComponent<Image>();
        button ??= GetComponent<Button>();
        
        if (!isEquipSlot)
            name ??= GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }    
#endif
}
