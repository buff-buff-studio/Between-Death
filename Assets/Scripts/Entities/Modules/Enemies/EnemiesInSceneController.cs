using System;
using System.Collections.Generic;
using UnityEngine;

namespace Refactor.Entities.Modules
{
    public class EnemiesInSceneController : MonoBehaviour
    {
        private List<GioEntityModule> enemiesToSpawn;
        private bool _hasSpawned;

        public void Spawn()
        {
            _hasSpawned = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player") && !_hasSpawned)
                Spawn();
        }
    }
}