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

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        
        skills ??= Resources.Load<SkillList>("Skills/SkillList");
    }

    private void AddSkill(int skill)
    {
        if (inventorySkills.Count < 6 && !inventorySkills.Contains(skill))
        {
            inventorySkills.Add(skill);
            UpdateInventory();
        }
    }
    
    private void ChangeSlot(int slot, int skill)
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
            equippedSlots[i].sprite = skills.GetIcon(skill);
            inGameSlots[i].sprite = skills.GetIcon(skill);
            i++;
        }
    }
    
    public bool IsEquipped(int id)
    {
        return equippedSkills.Contains(id);
    }
}
