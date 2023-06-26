using System;
using System.Collections;
using System.Collections.Generic;
using Refactor.Entities;
using Refactor.Entities.Modules;
using Refactor.Misc;
using UnityEngine;

namespace Refactor.Props
{
    public class Bell : MonoBehaviour
    {
        public Vector3 topPosition = new(0, 10, 40);
        public Vector3 bottomPosition = new Vector3(0, 0, 40);

        public bool state = false;
        
        public GameObject[] enemiesPrefabs;
        public int enemyCount = 5;
        public Vector3 respawnPosition = new Vector3(0, 0, 30);
        public Entity player;
        
        public List<Entity> aliveEnemies = new List<Entity>();
        
        public void ClimbTheBell()
        {
            state = true;
        }
        
        public void Update()
        {
            var deltaTime = Time.deltaTime;
            var targetPos = state ? topPosition : bottomPosition;
            transform.position = Vector3.Lerp(transform.position, targetPos, deltaTime);
        }

        public void SpawnEnemies()
        {
            ClimbTheBell();
            
            player.respawnPosition = respawnPosition;
            
            for (var i = 0; i < enemyCount; i++)
            {
                var pos = bottomPosition + Vector3.up + Quaternion.Euler(0, i * 360f / enemyCount, 0) * new Vector3(0, 0, 10);
                var go = Instantiate(enemiesPrefabs[i % enemiesPrefabs.Length], pos, Quaternion.identity);
                var entity = go.GetComponent<Entity>();
                var module = entity.GetModule<EnemyControllerEntityModule>();
                module.shouldAttack = true;
                module.attackDistance = 10f;
                module.attackTarget = player.transform;
                module.wanderingCenterPoint = bottomPosition + Vector3.up;
                module.NewTarget();
                aliveEnemies.Add(entity);
                
                entity.GetModule<HealthEntityModule>().onDie.AddListener(() => OnEnemyDie(entity));
            }
        }
        
        public void OnEnemyDie(Entity e)
        {
            if (!aliveEnemies.Contains(e)) return;
            aliveEnemies.Remove(e);
            
            if(aliveEnemies.Count == 0)
                StartCoroutine(_OnAllDied());
        }

        private IEnumerator _OnAllDied()
        {
            state = false;
            yield return new WaitForSeconds(5f);
            var module = player.GetModule<HealthEntityModule>() as IHealth;
            module.Heal(module.maxHealth);
        }
    }
}