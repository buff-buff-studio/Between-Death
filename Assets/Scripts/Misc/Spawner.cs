using System;
using Refactor.Entities;
using Refactor.Entities.Modules;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Refactor.Misc
{
    
    public class Spawner : MonoBehaviour
    {
        public Pool pool;

        public Transform[] spawnPositions;

        public GameObject[] prefabs;
        
        public int minCount = 4;
        public int maxCount = 6;
        [SerializeField, HideInInspector] 
        private int _count = 0;
        
        void Awake()
        {
            _count = Random.Range(minCount, maxCount + 1);
            pool.limitPerType = 10;

            for (int i = 0; i < _count; i++)
            {
                SpawnOther();
            }
        }

        public void RemoveEntity(Entity entity)
        {
            pool.Destroy(entity.gameObject);
            SpawnOther();
        }
        
        //Spawn next
        public void SpawnOther()
        {
            var prefab = prefabs[Random.Range(0, prefabs.Length)];
            var pos = spawnPositions[Random.Range(0, spawnPositions.Length)].position;
            
            Entity entity = pool.Instantiate<GameObject>(prefab, pos, Quaternion.Euler(0, Random.Range(0, 360), 0), transform, true).GetComponent<Entity>();
            GioEntityModule gioEntity = entity.GetModule<GioEntityModule>();
            if(gioEntity == null)
                return;
            entity.controller.enabled = false;
            entity.transform.position = pos;
            entity.controller.enabled = true;

            gioEntity.spawner = this;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            foreach (Transform pos in spawnPositions)
            {
                Gizmos.DrawWireSphere(pos.position, 0.25f);
            }
        }
    }
}