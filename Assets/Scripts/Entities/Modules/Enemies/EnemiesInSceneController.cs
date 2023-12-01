using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Refactor.Entities.Modules
{
    public class EnemiesInSceneController : Singleton<EnemiesInSceneController>
    {
        [SerializeField]
        private List<GioEntityModule> enemiesToSpawn;
        private bool _hasSpawned;
        [FormerlySerializedAs("_enemiesInScene")] [SerializeField]
        private List<GioEntityModule> enemiesInScene = new List<GioEntityModule>();
        private float time = 2;
        private float currentTime = 2;
        public bool routineAttacking;
        private GioEntityModule enemyAttacking;
        private int _currentIndex;
        public void Spawn()
        {
            _hasSpawned = true;

            foreach (var e in enemiesToSpawn)
            {
                // instantiate enemy and assign enemy controller
            }
        }

        public void StartRoutineAttacking()
        {
            if(!routineAttacking)return;
            if (enemiesInScene.Count == 1)
            {
                routineAttacking = false;
           //     NextEnemy();
            }
            if (currentTime > time)
            {
                currentTime = 0;
          //      NextEnemy();
            }
            currentTime += Time.fixedDeltaTime;
        }


        public bool HasMoreThanOne()
        {
            return enemiesInScene.Count > 1;
        }

        /*private void NextEnemy()
        {
            if (enemyAttacking != null)
                enemyAttacking.isGoingToAttack = false;

            if(enemiesInScene.Count == 0) return;
            
            enemyAttacking = enemiesInScene[_currentIndex % enemiesInScene.Count];
            enemyAttacking.isGoingToAttack = true;
            _currentIndex++;
        }*/

        public void NoMoreEnemies()
        {
            routineAttacking = false;
            _currentIndex = 0;
        }
        
        private void FixedUpdate()
        {
            StartRoutineAttacking();
        }
        

        public void AddEnemy(GioEntityModule entity)
        {
            enemiesInScene.Add(entity);
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player") && !_hasSpawned)
                Spawn();
        }

        public void RemoveEnemy(GioEntityModule entity)
        {
            if(enemiesInScene.Contains(entity))
                enemiesInScene.Remove(entity);
            
            if(enemiesInScene.Count == 0)
                NoMoreEnemies();
        }
        
    }
}