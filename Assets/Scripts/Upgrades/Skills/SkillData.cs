using Refactor.Data;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "Skill", menuName = "RPG/Skill")]
public class SkillData : GenericItemData
{
    public Animation animation;
    public VideoClip preview;
    
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