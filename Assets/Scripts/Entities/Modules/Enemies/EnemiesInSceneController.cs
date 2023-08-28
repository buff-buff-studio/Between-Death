using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Refactor.Entities.Modules
{
    public class EnemiesInSceneController : MonoBehaviour
    {
        [SerializeField]
        private List<GioEntityModule> enemiesToSpawn;
        private bool _hasSpawned;
        private List<GioEntityModule> _enemiesInScene = new List<GioEntityModule>();
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
            if(routineAttacking)return;
            routineAttacking = true;
            if (currentTime > time)
            {
                currentTime = 0;
                NextEnemy();
            }
            currentTime += Time.fixedDeltaTime;
        }

        private void NextEnemy()
        {
            if (enemyAttacking != null)
                enemyAttacking.isGoingToAttack = false;


            enemyAttacking = _enemiesInScene[_currentIndex % _enemiesInScene.Count];
            enemyAttacking.isGoingToAttack = true;
            _currentIndex++;
        }

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
            _enemiesInScene.Add(entity);
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player") && !_hasSpawned)
                Spawn();
        }

        public void RemoveEnemy(GioEntityModule entity)
        {
            if(_enemiesInScene.Contains(entity))
                _enemiesInScene.Remove(entity);
            
            if(_enemiesInScene.Count == 0)
                NoMoreEnemies();
        }
        
    }
}