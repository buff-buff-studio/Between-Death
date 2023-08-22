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
    public string GetName(int i) => passives[i].name;
    public string GetDescription(int i) => passives[i].description;
    
    //Visuals
    public Sprite GetIcon(int i) => passives[i].icon;
    
    //Values
    public PassiveData.PassiveType GetType(int i) => passives[i].type;
    public float GetPercentage(int i) => passives[i].percentage;
    public int GetTime(int i) => (int)passives[i].time;
}
