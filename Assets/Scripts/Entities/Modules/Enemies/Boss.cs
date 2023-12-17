using System;
using System.Collections;
using System.Collections.Generic;
using Refactor.Data;
using Refactor.Entities;
using Refactor.Entities.Modules;
using Refactor.Misc;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.GlobalIllumination;
using Random = UnityEngine.Random;

[Serializable]
public class Boss : GioEntityModule
{
    private float amountOfStages = 3;
    private int amountToSpawn = 7;
    private int amountToSpawnEnemies = 4;
    private int range = 20;
    private List<Entity> trees = new List<Entity>();
    [SerializeField]
    private Entity spawnObject;

    private int currentCountOfTrees = 0;
    [SerializeField]
    private float healSpeed;

    private bool hasSpawnedTrees;
    [SerializeField]
    private List<GameObject> enemiesToSpawn = new List<GameObject>();

    private int currentStage = 1;

    private float[] currentHealthStage = new float[3];

    private float heathDif;
    
    //private List<GioEntityModule> currentEntities = new List<GioEntityModule>();
    [SerializeField]
    private Light light;
    [SerializeField]
    private Color chaosColor, orderColor;
    [SerializeField]
    private NavMeshSurface meshSurface;

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

    public override void OnEnable()
    {
        base.OnEnable();
        SetHealth();
        ChangeLightColor();
    }

    private void ChangeLightColor()
    {
        light.color = entity.element == Element.Chaos ? chaosColor : orderColor;
    }

    private void SetHealth()
    {
        var before = 0;
        var healthAux = hm._health.value - 2;
        heathDif = healthAux / amountOfStages;
        for (int i = 0; i < amountOfStages; i++)
        {
            Debug.Log("a");
            currentHealthStage[i] = healthAux - heathDif;
            healthAux = currentHealthStage[i];
        }
    }
    
    protected override void Special()
    {
       base.Special();
    
       IHealth ihm = (IHealth) hm;
       ihm.Heal(hm._maxHealth*.01f);

       if (hm._health >= hm._maxHealth)
           BackToState();
       
    }
    public override void OnEnemyTakeDamage(float amount)
    {
        //state = State.TakingDamage;
        if(state == State.Special) return;
        if(currentStage > amountOfStages) return;
        Debug.Log("TakingDamage");
        animator.CrossFade("Reaction", 0.25f);
        
        if (hm._health < currentHealthStage[currentStage-1])
        {
            Debug.Log("StateSpecial");
            state = State.Special;
            hm.enabled = false;
            hm.canTakeDamage = true;
            stateTime = 0;
            SpawnSpecial();
        }
    }
    
    protected override void TargetingState()
    {
       base.TargetingState();
       
       if (Math.Abs ((hm._health - currentHealthStage[currentStage-1]) ) < 1)
       {
           state = State.Special;
         
           hm.enabled = false;
           stateTime = 0;
           hm.canTakeDamage = true;
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

    private Element GetRandomElement()
    {
        return Random.Range(0, 2) == 0 ? Element.Order : Element.Chaos;
    }
    
    private void SpawnSpecial()
    {
        if(hasSpawnedTrees) return;
        hasSpawnedTrees = true;
        hm.canTakeDamage = false;
        currentStage++;
        currentCountOfTrees = amountToSpawn;
        var amount = Random.Range(amountToSpawn -2, amountToSpawn + 1);
        for (int i = 0; i < amountToSpawn; i++)
        {
            var pos = RandomPoint(entity.transform.position, range);
            var e = GameObject.Instantiate(spawnObject, pos, Quaternion.identity);
            e.GetModule<HealthEntityModule>().onDie.AddListener(OnKillTree);
            trees.Add(e);
            e.element = GetRandomElement();
        }

        amountToSpawnEnemies *= currentStage;
        
        for (int i = 0; i < amountToSpawnEnemies; i++)
        {
            var pos = RandomPoint(entity.transform.position, range);
            var e = GameObject.Instantiate(enemiesToSpawn[Random.Range(0,enemiesToSpawn.Count)], pos,Quaternion.identity);
            e.GetComponent<Entity>().element = GetRandomElement();
        }
        meshSurface.BuildNavMesh();
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
        //  currentEntities.Clear();
        hm.canTakeDamage = true;
        trees.Clear();
    }
}
