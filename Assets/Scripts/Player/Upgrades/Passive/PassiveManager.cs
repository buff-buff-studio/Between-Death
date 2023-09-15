using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Refactor.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class PassiveManager : MonoBehaviour
{
    public static PassiveManager instance;
    
    [Header("Passives")]
    [NotNull] public PassiveList passives;
    [SerializeField] private int[] equippedPassives = new int[2];
    
    [Space]
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI passiveName;
    [SerializeField] private TextMeshProUGUI passiveDescription;
    
    [Space]
    [Header("Slots")]
    [SerializeField] private PassiveSlot[] inventorySlots = new PassiveSlot[9];
    // 0 = Order
    // 1 = Chaos
    [SerializeField] private InputSlot[] equippedSlots = new InputSlot[2];
    
    private int _selectedPassive = -1;
    private int _infoPassive = -1;
    
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        
        passives ??= Resources.Load<PassiveList>("Passive/PassiveList");
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
    
    public void UpdateInfo(int skill)
    {
        if(_infoPassive >= 0) inventorySlots.ToList().Find(x => x.ID == _infoPassive).hover.enabled = false;
        passiveName.text = passives.GetName(skill);
        passiveDescription.text = passives.GetDescription(skill);
        _infoPassive = skill;
        
        inventorySlots.ToList().Find(x => x.ID == skill).hover.enabled = true;
    }
    
    private void ChangeSlot(uint slot, int skill)
    {
        if (equippedPassives.Contains(skill))
        {
            //find the index 
            int oldSlot = Array.IndexOf(equippedPassives, skill);
            equippedPassives[oldSlot] = equippedPassives[slot];
        }
        equippedPassives[slot] = skill;
        UpdateEquipped();
        UpdateInventory();
    }

    private void UpdateInventory()
    {
        foreach (var passive in inventorySlots)
        {
            passive.UpdateSlot();
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

    public bool IsEquipped(int id)
    {
        return equippedPassives.Contains(id);
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
