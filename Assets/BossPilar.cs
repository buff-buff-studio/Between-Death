using System;
using System.Collections;
using System.Collections.Generic;
using Refactor.Data;
using Refactor.Entities;
using Refactor.Entities.Modules;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class BossPilar : Entity
{
    [SerializeField]
    private Renderer mat;
    [SerializeField]
    private Material[] elementsMaterial = new Material[2];

   
    
    public override void OnEnable()
    {
        base.OnEnable();
        onChangeElement.AddListener(OnChangeElement);
        GetModule<HealthEntityModule>().onDie.AddListener(OnDie);
    }
    
    public override void OnDisable()
    {
        base.OnDisable();
        onChangeElement.RemoveListener(OnChangeElement);
        GetModule<HealthEntityModule>().onDie.RemoveListener(OnDie);
    }

    private void OnDie()
    {
        Destroy(gameObject);
    }

    private void OnChangeElement()
    {
        mat.material = elementsMaterial[element == Element.Chaos ? 0 : 1];
    }
}
