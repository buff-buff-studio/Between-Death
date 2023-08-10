using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [SerializeField] [NotNull] 
    private SkillList skillList;

    [SerializeField] private List<int> inventorySkills;
    [SerializeField] private int[] equippedSkills = new int[3];
}
