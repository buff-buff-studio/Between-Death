using System.Collections;
using System.Collections.Generic;
using Refactor.Data;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillList", menuName = "RPG/SkillList")]
public class SkillList : ScriptableObject
{
    public List<SkillData> skills = new List<SkillData>();
    
    public SkillData Get(int i) => skills[i];
    public Element GetElement(int i) => skills[i].element;
    public int GetDamage(int i) => skills[i].damage;
    public float GetDuration(int i) => skills[i].duration;
    public float GetCooldown(int i) => skills[i].cooldown;
    public Sprite GetIcon(int i) => skills[i].icon;
    public Animation GetAnimation(int i) => skills[i].animation;
    
    public bool IsElement(int i, Element element) => skills[i].element == element;
}
