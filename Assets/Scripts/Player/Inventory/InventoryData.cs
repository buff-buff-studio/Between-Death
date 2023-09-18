using System.Collections;
using System.Collections.Generic;
using Refactor.Data;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Refactor/Data/Inventory", order = 1)]
public class InventoryData : ScriptableObject
{
    [Header("Skills")]
    [SerializeField] private List<int> unlockedSkills = new List<int>(9);
    [SerializeField] private List<int> equippedSkills = new List<int>(3);
    
    [Space]
    [Header("Passives")]
    [SerializeField] private List<int> unlockedPassives = new List<int>(9);
    [SerializeField] private int orderPassive;
    [SerializeField] private int chaosPassive;
    
    [Space]
    [Header("Documents")]
    [SerializeField] private List<int> unlockedDocuments;
    
    public int GetEquippedSkill(int index) => equippedSkills.Count < index+1 ? -1 : equippedSkills[index];
    public int GetUnlockedSkill(int index) => unlockedSkills.Count < index+1 ? -1 : unlockedSkills[index];
    public int GetEquippedPassive(Element element) => element == Element.Order ? orderPassive : chaosPassive;
    
    public List<int> GetEquippedSkills => equippedSkills;
    
    public List<int> GetUnlockedSkills => unlockedSkills;
    public List<int> GetUnlockedPassives => unlockedPassives;
    public List<int> GetUnlockedDocuments => unlockedDocuments;
    
    public void AddUnlockedSkill(int skillId)
    {
        if(unlockedSkills.Contains(skillId)) return;
        unlockedSkills.Add(skillId);
    }
    public void SetEquippedSkill(int index, int skillId)
    {
        if(!unlockedSkills.Contains(skillId) || index > 2) return;
        equippedSkills[index] = skillId;
    }
    public void ChangeEquippedSkill(int index, int index2, int skillId)
    {
        if(!equippedSkills.Contains(skillId) || index > 2 || index2 > 2) return;
        equippedSkills[index2] = equippedSkills[index];
        equippedSkills[index] = skillId;
    }
    
    public void AddUnlockedPassive(int passiveId)
    {
        if(unlockedPassives.Contains(passiveId)) return;
        unlockedPassives.Add(passiveId);
    }
    public void SetEquippedPassive(Element element, int passiveId)
    {
        if(!unlockedPassives.Contains(passiveId)) return;
        if(element == Element.Order) orderPassive = passiveId;
        else chaosPassive = passiveId;
    }
    
    public void AddUnlockedDocument(int documentId)
    {
        if(unlockedDocuments.Contains(documentId)) return;
        unlockedDocuments.Add(documentId);
    }
}
