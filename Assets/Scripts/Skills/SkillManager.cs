using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;
    
    [FormerlySerializedAs("Skills")]
    [Header("Skills")]
    [NotNull] public SkillList skills;

    [SerializeField] private List<int> inventorySkills;
    [SerializeField] private int[] equippedSkills = new int[3];
    
    [Space]
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private Image skillPreview;
    [SerializeField] private TextMeshProUGUI skillElement;
    [SerializeField] private TextMeshProUGUI skillDescription;
    
    [Space]
    [Header("Slots")]
    [SerializeField] private SkillItem[] inventorySlots = new SkillItem[6];
    [SerializeField] private SkillItem[] equippedSlots = new SkillItem[3];
    [SerializeField] private Image[] inGameSlots = new Image[3];

    private int _selectedSkill = -1;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        
        skills ??= Resources.Load<SkillList>("Skills/SkillList");
        
        
    }

    public void UpdateInfo(int skill)
    {
        skillName.text = skills.GetName(skill);
        skillPreview.sprite = skills.GetPreview(skill);
        skillElement.text = skills.GetElement(skill).ToString();
        skillDescription.text = skills.GetDescription(skill);
    }

    private void AddSkill(int skill)
    {
        if (inventorySkills.Count < 6 && !inventorySkills.Contains(skill))
        {
            inventorySkills.Add(skill);
            UpdateInventory();
        }
    }
    
    private void ChangeSlot(uint slot, int skill)
    {
        if (inventorySkills.Contains(skill))
        {
            equippedSkills[slot] = skill;
            UpdateEquipped();
            UpdateInventory();
        }
    }

    private void UpdateInventory()
    {
        var i = 0;
        foreach (var skill in inventorySkills)
        {
            if(equippedSkills.Contains(skill)) continue;
            inventorySlots[i].sprite = skills.GetIcon(skill);
            i++;
        }
    }
    
    private void UpdateEquipped()
    {
        var i = 0;
        foreach (var skill in equippedSkills)
        {
            equippedSlots[i].UpdateSkill((uint)skill);
            inGameSlots[i].sprite = skills.GetIcon(skill);
            i++;
        }
    }

    public bool IsEquipped(uint id)
    {
        return equippedSkills.Contains((int)id);
    }

    public bool InInventory(uint id)
    {
        return inventorySkills.Contains((int)id);
    }

    public void Equip(uint id, uint slot)
    {
        if (_selectedSkill < 0)
        {
            _selectedSkill = (int)id;
        }
        else
        {
            ChangeSlot(slot,_selectedSkill);
            _selectedSkill = -1;
        }
    }

    public void Select(uint id)
    {
        _selectedSkill = (int)id;
        UpdateInfo((int)id);
    }
}
