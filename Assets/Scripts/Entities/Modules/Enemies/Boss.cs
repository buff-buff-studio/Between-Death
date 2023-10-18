using System;
using System.Collections;
using System.Collections.Generic;
using Refactor.Entities.Modules;
using UnityEngine;

[Serializable]
public class Boss : GioEntityModule
{
    private int amountOfStages = 3;

    private GameObject spawnObject;
    
    public virtual void OnEnemyTakeDamage(float amount)
    {
        //state = State.TakingDamage;
        Debug.Log("TakingDamage");
        animator.CrossFade("Reaction", 0.25f);
        
        
        if (Math.Abs ((hm._health % amountOfStages) - 0) <= 1)
        {
            state = State.Special;
            hm.enabled = false;
            SpawnSpecial();
        }
    }

    private void SpawnSpecial()
    {
        
    }
}
