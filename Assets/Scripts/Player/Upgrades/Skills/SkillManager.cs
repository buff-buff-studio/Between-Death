using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Refactor.Data;
using Refactor.Data.Variables;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Video;

[HelpURL("https://www.canva.com/design/DAFbnGKaHOw/RkEvLyhFUpMgglFu3D0jWw/edit")]
public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;
    
    [Header("Skills")]

    [SerializeField] private Variable<InventoryData> inventory;

    private InventoryData inventoryData => inventory.Value;
    public SkillList skills => inventoryData.GetSkillList;
    private List<int> inventorySkills => inventoryData.GetUnlockedSkills;
    private List<int> equippedSkills => inventoryData.GetEquippedSkills;
    
    [Space]
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private VideoPlayer skillPreview;
    [SerializeField] private TextMeshProUGUI skillElement;
    [SerializeField] private TextMeshProUGUI skillDescription;
    
    [Space]
    [Header("Slots")]
    [SerializeField] private List<SkillSlot> inventorySlots = new List<SkillSlot>(6);
    [SerializeField] private InputSlot[] equippedSlots = new InputSlot[3];
    [SerializeField] private List<SkillSlot> extraSlots = new List<SkillSlot>(3);


    [SerializeField]
    private int _selectedSkill = -1;
    private int _infoSkill = -1;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        UpdateInfo(inventoryData.GetEquippedSkill(0));
        UpdateExtras();
        UpdateEquipped();
        UpdateInventory();
        PassiveManager.instance?.UpdateExtras();
    }

    public void UpdateSkillUI()
    {
        UpdateInfo();
        UpdateEquipped();
        UpdateInventory();
    }

    public void UpdateInfo()
    {
        UpdateInfo(inventoryData.GetUnlockedSkill(0));
    }

    public void UpdateInfo(int skill)
    {
        if(skill < 0)
        {
            skillName.text = "";
            skillPreview.gameObject.SetActive(false);
            skillElement.text = "";
            skillDescription.text = "";
            return;
        }
        if (_infoSkill >= 0) inventorySlots.ToList().Find(x => x.ID == _infoSkill).hover.enabled = false;
        skillName.text = skills.GetName(skill);
        skillPreview.gameObject.SetActive(true);
        skillPreview.clip = skills.GetPreview(skill);
        skillElement.text = skills.GetElement(skill) switch
        {
            Element.Chaos => "Caos",
            Element.Order => "Ordem",
            Element.None => "Neutro",
            _ => skillElement.text
        };
        skillDescription.text = skills.GetDescription(skill);
        _infoSkill = skill;
        
        inventorySlots.ToList().Find(x => x.ID == skill).hover.enabled = true;
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
            if (equippedSkills.Contains(skill))
            {
                //find the index 
                int oldSlot = equippedSkills.IndexOf(skill);
                inventoryData.ChangeEquippedSkill((int)slot, oldSlot, skill);
            }else inventoryData.SetEquippedSkill((int)slot,skill);

            UpdateEquipped();
            UpdateInventory();
        }
    }

    private void UpdateInventory()
    {
        inventorySlots.ForEach(slt => slt.UpdateSlot(-1));
        var i = 0;
        foreach (var skill in inventorySkills)
        {
            inventorySlots[i].UpdateSlot(skill);
            i++;
        }
    }
    
    private void UpdateEquipped()
    {
        var i = 0;
        foreach (var skill in equippedSkills)
        {
            equippedSlots[i].UpdateSlot(skill);
            i++;
        }
        
        InGameHUD.instance?.UpdateSkillSlots();
    }

    public void UpdateExtras()
    {
        extraSlots.ForEach(slt => slt.UpdateSlot(-1));
        var i = 0;
        foreach (var skill in equippedSkills)
        {
            extraSlots[i].UpdateSlot(skill);
            i++;
        }
    }

    public bool IsEquipped(int id)
    {
        return equippedSkills.Contains(id);
    }

    public bool InInventory(int id)
    {
        return inventorySkills.Contains(id);
    }

    public void Equip(int id, uint slot)
    {
        if (_selectedSkill < 0)
        {
            Select(id);
        }
        else
        {
            ChangeSlot(slot,_selectedSkill);
            _selectedSkill = -1;
        }
    }

    public void Select(int id)
    {
        _selectedSkill = id;
        UpdateInfo(id);
    }
}
