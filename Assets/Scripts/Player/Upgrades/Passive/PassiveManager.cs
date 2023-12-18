using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Refactor.Data;
using Refactor.Data.Variables;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class PassiveManager : MonoBehaviour
{
    public static PassiveManager instance;

    [SerializeField] private Variable<InventoryData> inventory;

    private InventoryData inventoryData => inventory.Value;

    public PassiveList passives => inventoryData.GetPassiveList;
    private List<int> inventoryPassives => inventoryData.GetUnlockedPassives;
    private List<int> equippedPassives => inventoryData.GetEquippedPassives;
    
    [Space]
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI passiveName;
    [SerializeField] private TextMeshProUGUI passiveDescription;
    
    [Space]
    [Header("Slots")]
    [SerializeField] private List<PassiveSlot> inventorySlots = new List<PassiveSlot>(9);
// 0 = Order
    // 1 = Chaos
    [SerializeField] private InputSlot[] equippedSlots = new InputSlot[2];
    [SerializeField] private List<PassiveSlot> extraSlots = new List<PassiveSlot>(2);
    
    private int _selectedPassive = -1;
    private int _infoPassive = -1;
    
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        UpdateInfo(equippedPassives[0]);
        UpdateInventory();
        UpdateEquipped();
        UpdateExtras();
        SkillManager.instance?.UpdateExtras();
    }

    private void Start()
    {
        SetElements();
    }
    
    public void SetElements(bool active = false) { foreach (var passive in passives.passives) passive.SetEnable(active); }

    public void UpdateElement(Element element)
    {
        SetElements();
        passives.SetEnable(equippedPassives[0],element == Element.Order);
        passives.SetEnable(equippedPassives[1],element == Element.Chaos);
    }
    
    public void UpdateInfo(int passive)
    {
        if(passive < 0)
        {
            passiveName.text = "";
            passiveDescription.text = "";
            return;
        }
        if(_infoPassive >= 0)
            inventorySlots.Find(x => x.ID == _infoPassive).hover.enabled = false;

        passiveName.text = passives.GetName(passive);
        passiveDescription.text = passives.GetDescription(passive);
        _infoPassive = passive;
        
        inventorySlots.ToList().Find(x => x.ID == passive).hover.enabled = true;
    }
    
    private void ChangeSlot(uint slot, int passive)
    {
        if (equippedPassives.Contains(passive))
        {
            //find the index 
            int oldSlot = equippedPassives.FindIndex(x => x == passive);
            equippedPassives[oldSlot] = equippedPassives[(int)slot];
        }
        equippedPassives[(int)slot] = passive;
        UpdateEquipped();
        UpdateInventory();
    }

    private void UpdateInventory()
    {
        inventorySlots.ForEach(slt => slt.UpdateSlot(-1));
        var i = 0;
        foreach (var passive in inventoryPassives)
        {
            inventorySlots[i].UpdateSlot(passive);
            i++;
        }
    }
    
    private void UpdateEquipped()
    {
        var i = 0;
        foreach (var passive in equippedPassives)
        {
            equippedSlots[i].UpdateSlot(passive);
            i++;
        }
    }

    public void UpdateExtras()
    {
        extraSlots.ForEach(slt => slt.UpdateSlot(-1));
        var i = 0;
        foreach (var passive in equippedPassives)
        {
            extraSlots[i].UpdateSlot(passive);
            i++;
        }
    }

    public bool IsEquipped(int id)
    {
        return equippedPassives.Contains(id);
    }

    public bool InInventory(int id)
    {
        return inventoryPassives.Contains(id);
    }

    public void Equip(int id, uint slot)
    {
        if (_selectedPassive < 0)
        {
            Select(id);
        }
        else
        {
            ChangeSlot(slot,_selectedPassive);
            _selectedPassive = -1;
        }
    }

    public void Select(int id)
    {
        _selectedPassive = id;
        UpdateInfo(id);
    }
}
