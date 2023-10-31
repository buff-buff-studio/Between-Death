using System;
using System.Collections;
using System.Collections.Generic;
using Refactor.Data;
using Refactor.Entities;
using Refactor.Entities.Modules;
using Refactor.Misc;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[Serializable]
public class Boss : GioEntityModule
{
    private int amountOfStages = 3;
    private int amountToSpawn = 10;
    private int range = 10;
    private List<Entity> trees = new List<Entity>();
    [SerializeField]
    private Entity spawnObject;

    private int currentCountOfTrees = 0;
    [SerializeField]
    private float healSpeed;

    private bool hasSpawnedTrees;

    public void OnKillTree()
    {
        Debug.Log("OnKillTree");
        currentCountOfTrees--;

        if (currentCountOfTrees <= 0)
            BackToState();
    }

    protected override Vector3 SpecialPos()
    {
        return Vector3.zero;
    }

    protected override void Special()
    {
       base.Special();
    
       IHealth ihm = (IHealth) hm;
       ihm.Heal(Time.deltaTime * healSpeed);

       if (hm._health >= hm._maxHealth)
       {
           BackToState();
       }
       
    }
    public override void OnEnemyTakeDamage(float amount)
    {
        //state = State.TakingDamage;
        if(state == State.Special) return;
        Debug.Log("TakingDamage");
        animator.CrossFade("Reaction", 0.25f);
        
        if (Math.Abs ((hm._health % amountOfStages) - 0) < 1)
        {
            state = State.Special;
            hm.enabled = false;
            stateTime = 0;
            SpawnSpecial();
        }
    }
    
    protected override void TargetingState()
    {
       base.TargetingState();
       
        
       if (Math.Abs ((hm._health % amountOfStages) - 0) < 1)
       {
           state = State.Special;
           hm.enabled = false;
           stateTime = 0;
           SpawnSpecial();
       }
    }

    private void BackToState()
    {
        if(!hasSpawnedTrees) return;
        Debug.Log("BackToState");
        hasSpawnedTrees = false;
        ClearSpecial();
        state = State.Targeting;
        hm.enabled = true;
        stateTime = 0;
        entity.element = entity.element == Element.Chaos ? Element.Order : Element.Chaos;
    }
    
    
    private void SpawnSpecial()
    {
        if(hasSpawnedTrees) return;
        hasSpawnedTrees = true;
        currentCountOfTrees = amountToSpawn;
        for (int i = 0; i < amountToSpawn; i++)
        {
            var pos = RandomPoint(entity.transform.position, range);
            var e = GameObject.Instantiate(spawnObject, pos,Quaternion.identity);
            e.GetModule<HealthEntityModule>().onDie.AddListener(OnKillTree);
            trees.Add(e);
            var randomElement = Random.Range(1, 3);
            e.element = (Element)randomElement;
        }
    }

    public override void UpdateFrame(float deltaTime)
    {
        base.UpdateFrame(deltaTime);
        if (Input.GetKeyDown(KeyCode.B))
            SpawnSpecial();
        
    }

    private Vector3 RandomPoint(Vector3 center, float range)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        return NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas) ? hit.position : RandomPoint(center, range);
    }

    private void ClearSpecial()
    {
        foreach (var t in trees)
        {
            if (t == null) continue;
            if(t.gameObject)
                GameObject.Destroy(t.gameObject);

        }
            
        
        
        trees.Clear();
    }
}
