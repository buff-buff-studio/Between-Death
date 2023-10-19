using System;
using System.Collections;
using System.Collections.Generic;
using Refactor.Data;
using Refactor.Entities;
using Refactor.Entities.Modules;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[Serializable]
public class Boss : GioEntityModule
{
    private int amountOfStages = 3;
    private int amountToSpawn = 10;
    private List<Entity> trees = new List<Entity>();
    [SerializeField]
    private Entity spawnObject;

    private int currentCountOfTrees = 0;
    public override void OnEnable()
    {
        base.OnEnable();
        
        spawnObject.GetModule<HealthEntityModule>().onDie.AddListener(OnKillTree);

    }


    public void OnKillTree()
    {
        currentCountOfTrees--;

        if (currentCountOfTrees <= 0)
        {
            BackToState();
        }
    }
    
    public virtual void OnEnemyTakeDamage(float amount)
    {
        //state = State.TakingDamage;
        Debug.Log("TakingDamage");
        animator.CrossFade("Reaction", 0.25f);
        
        
        if (Math.Abs ((hm._health % amountOfStages) - 0) <= 1)
        {
            state = State.Special;
            hm.enabled = false;
            stateTime = 0;
            SpawnSpecial();
        }
    }

    private void BackToState()
    {
        state = State.Targeting;
        hm.enabled = true;
        stateTime = 0;
    }
    
    
    private void SpawnSpecial()
    {
        currentCountOfTrees = amountToSpawn;
        for (int i = 0; i < amountToSpawn; i++)
        {
            var pos = RandomPoint(entity.transform.position, 100);
            var e = GameObject.Instantiate(spawnObject, pos,Quaternion.identity);
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
            GameObject.Destroy(t);
        
        trees.Clear();
    }
}
