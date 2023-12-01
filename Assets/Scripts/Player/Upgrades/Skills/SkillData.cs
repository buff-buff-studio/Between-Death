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
    public float cooldown;

    [Header("Cooldown")]
    public float actualCooldown = 0;
    
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
        attack.nextAttackWindow = 0.9f;
        return attack;
    }
    
    public override void OpenItem()
    {
        InGameHUD.instance.OpenSkill(this);
    }
}
