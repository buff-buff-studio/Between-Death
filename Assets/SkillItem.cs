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
    }

    public void UpdateSkill(int id)
    {
        this.id = id;
        icon.sprite = SkillManager.instance.skills.GetIcon(id);
        button.interactable = SkillManager.instance.IsEquipped(id);
        
        if (!isEquipSlot) 
            name.text = SkillManager.instance.skills.GetName(id);
    }

    private void OnValidate()
    {
        icon ??= GetComponent<Image>();
        button ??= GetComponent<Button>();
        
        if (!isEquipSlot)
            name ??= GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }
}
