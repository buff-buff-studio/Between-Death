using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "Passive", menuName = "RPG/Passive")]
public class PassiveData : ScriptableObject
{
    [Header("About")]
    public string name;
    [TextArea(3, 10)]
    public string description;
    
    [Space] [Header("Visuals")]
    public Sprite icon;

    [Space] [Header("Values")] 
    public PassiveType type;
    [Range(0, 100)] public half percentage;
    [Range(0, 60)] public uint time;

    public enum PassiveType
    {
        AttackRange,
        DashRange,
        DeathRegen,
        RegenKill,
        MovementVelocity,
        ElementalDamage,
        ComboMultiplier
    }

    /*
     * death defy - regenera X% da vida ao morrer (1 vez por vida)
     * weapon range - aumenta alcance de todos os ataques básicos em X%
     * dash range - aumenta distância da esquiva em X%
     * light steps - aumenta velocidade de movimento em X%
     * elemental affinity - aumenta o dano causado ao mesmo elemento em X% porém diminue dano ao elemento oposto igualmente
     * life drain - fortalece a o roubo de vida do último ataque do combo em X%
     * torturer - o multiplicador do combo dura mais tempo
     */
}
