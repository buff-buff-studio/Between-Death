using System.Collections;
using System.Collections.Generic;
using Refactor.Data;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "SkillList", menuName = "RPG/SkillList")]
public class SkillList : ScriptableObject
{
    public List<SkillData> skills = new List<SkillData>();
    
    public SkillData Get(int i) => skills[i];
    
    //About
    public string GetName(int i) => skills[i].name;
    public string GetDescription(int i) => skills[i].description;
    
    //Visuals
    public Sprite GetIcon(int i) => skills[i].icon;
    public Animation GetAnimation(int i) => skills[i].animation;
    public VideoClip GetPreview(int i) => skills[i].preview;
    
    //Values
    public Element GetElement(int i) => skills[i].element;
    public int GetDamage(int i) => skills[i].damage;
    public int GetKnockback(int i) => skills[i].knockback;
    public float GetDuration(int i) => skills[i].duration;
    public float GetCooldown(int i) => skills[i].cooldown;
    
    public bool IsElement(int i, Element element) => skills[i].element == element;
}