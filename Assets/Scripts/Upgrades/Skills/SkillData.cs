using Refactor.Data;
using Refactor.Entities;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "Skill", menuName = "RPG/Skill")]
public class SkillData : GenericItemData
{
    public VideoClip preview;
    
    [Space]
    [Header("Values")]
    public Element element;
    [Range(0,60f)] [Tooltip("Cooldown in seconds")]
    public float cooldown;
    
    [Header("Attack")]
    public string clipName;
    public float damage = 1f;
    
    [Header("Timings")]
    public float transitionTime = 0.1f;
    public float damageTime = 0.5f;
    
    public Attack GetAttack()
    {
        var attack = CreateInstance<Attack>();
        attack.clipName = clipName;
        attack.damage = damage;
        attack.transitionTime = transitionTime;
        attack.damageTime = damageTime;
        return attack;
    }
}
