using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    [Header("Skills")]
    [SerializeField] [NotNull] 
    private SkillList skillList;

    [SerializeField] private List<int> inventorySkills;
    [SerializeField] private int[] equippedSkills = new int[3];
    
    [Space]
    [Header("UI")]
    [SerializeField] private Image[] inventorySlots = new Image[6];
    [SerializeField] private Image[] equippedSlots = new Image[3];
    [SerializeField] private Image[] inGameSlots = new Image[3];
    
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
            inventorySlots[i].sprite = skillList.GetIcon(skill);
            i++;
        }
    }
    
    private void UpdateEquipped()
    {
        var i = 0;
        foreach (var skill in equippedSkills)
        {
            equippedSlots[i].sprite = skillList.GetIcon(skill);
            inGameSlots[i].sprite = skillList.GetIcon(skill);
            i++;
        }
    }
}
