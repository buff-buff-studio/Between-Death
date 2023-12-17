using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "PassiveList", menuName = "RPG/Passive List")]
public class PassiveList : ScriptableObject
{
    public List<PassiveData> passives = new List<PassiveData>();

    public PassiveData Get(int i) => passives[i];
    
    //About
    public string GetName(int i) => i < 0 ? "" : passives[i].name;
    public string GetDescription(int i) => i < 0 ? "" : passives[i].description;
    
    //Visuals
    public Sprite GetIcon(int i) => i < 0 ? null : passives[i].icon;
    
    //Values
    public float GetModifier(int i) => i < 0 ? 1f : passives[i].modifier;
    public void SetEnable(int i, bool enable) => passives[i].SetEnable(enable);
}
