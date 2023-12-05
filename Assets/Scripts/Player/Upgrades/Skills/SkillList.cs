using System.Collections;
using System.Collections.Generic;
using Refactor.Data;
using Refactor.Entities;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "SkillList", menuName = "RPG/SkillList")]
public class SkillList : ScriptableObject
{
    public List<SkillData> skills = new List<SkillData>();
    
    public SkillData Get(int i) => skills[i];
    
    //About
    public int GetID(SkillData item) => skills.IndexOf(item);
    public string GetName(int i) => i < 0 ? "" : skills[i].name;
    public string GetDescription(int i) => i < 0 ? "" : skills[i].description;
    
    //Visuals
    public Sprite GetIcon(int i) => i < 0 ? null : skills[i].icon;
    public VideoClip GetPreview(int i) => i < 0 ? null : skills[i].preview;
    
    //Values
    public Element GetElement(int i) => i < 0 ? Element.None : skills[i].element;
    public bool IsElement(int i, Element element) => skills[i].element == element;
}
