using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Refactor.Data;
using UnityEngine;

public class PassiveManager : MonoBehaviour
{
    public static PassiveManager instance;
    
    [Header("Passives")]
    [NotNull] public PassiveList passives;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        
        passives ??= Resources.Load<PassiveList>("Passive/PassiveList");
    }

    public static void UpdateElement(Element element)
    {
        instance.passives.SetEnable(0,element == Element.Chaos);
    }
}
