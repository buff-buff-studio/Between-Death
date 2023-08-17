using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillItem : MonoBehaviour
{
    [SerializeField] private bool isEquipSlot = false;
    
    [SerializeField] private int id;
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

    public void UpdateSkill(int id)
    {
        this.id = id;
        icon.enabled = true;
        
        if (!isEquipSlot)
        {
            gameObject.SetActive(SkillManager.instance.InInventory(id));
            icon.sprite = SkillManager.instance.skills.GetIcon(id);
            button.interactable = !SkillManager.instance.IsEquipped(id);
            name.text = SkillManager.instance.skills.GetName(id);
        }
        else
        {
            if (id >= 0) icon.sprite = SkillManager.instance.skills.GetIcon(id);
            else icon.enabled = false;
            button.interactable = true;
        }
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
