using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenu : MonoBehaviour
{
    [Header("Skills References")]
    [SerializeField] private SkillManager skillManager;
    
    [Space]
    [Header("Passive References")]
    [SerializeField] private PassiveManager passiveManager;
    
    [Space]
    [Header("Document References")]
    [SerializeField] private InspectDoc documentInspect;
}
