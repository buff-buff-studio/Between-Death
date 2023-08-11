using System.Collections;
using System.Collections.Generic;
using Refactor.Data;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "RPG/Skill")]
public class SkillData : ScriptableObject
{
    [Header("Visuals")]
    public Animation animation;
    public Sprite icon;
    
    [Space]
    [Header("Values")]
    public Element element;
    public int damage;
    public int knockback;
    [Range(0,10f)] [Tooltip("Duration in seconds")]
    public float duration;
    [Range(0,60f)] [Tooltip("Cooldown in seconds")]
    public float cooldown;
}
