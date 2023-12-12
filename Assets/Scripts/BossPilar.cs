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

    private static readonly int OrderChaos = Shader.PropertyToID("_Order_Chaos");
    [SerializeField]
    private ParticleSystem particleSystem;
    
    
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
        mat.material.SetFloat(OrderChaos,element == Element.Chaos ? 1 : 0);
        ParticleSystem.MainModule settings = particleSystem.main;
        settings.startColor = new ParticleSystem.MinMaxGradient( element == Element.Chaos ? Color.red : Color.blue );
    }
}
