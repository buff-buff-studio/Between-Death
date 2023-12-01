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
        public Material material;
        public float dissolvingTime = 2f;

        public bool state = false;
        
        public GameObject[] enemiesPrefabs;
        public int enemyCount = 5;
        public Vector3 respawnPosition = new Vector3(0, 0, 30);
        public Entity player;
        
        public List<Entity> aliveEnemies = new List<Entity>();
        private static readonly int Dissolve = Shader.PropertyToID("_Dissolve");

        [SerializeField]
        private SkillData skill;
        [SerializeField]
        private uint ordersEnemies = 0;
        [SerializeField]
        private uint ordersToWin = 3;

        private void OnEnable()
        {
            material.SetFloat(Dissolve, 0);
        }

        public void ClimbTheBell()
        {
            state = true;
        }
        
        public void Update()
        {
            var deltaTime = Time.deltaTime;
        }

        public void SpawnEnemies()
        {
            if (state) return;

            ClimbTheBell();
            StartCoroutine(BellDissolve(true));
            var enCount = (enemyCount * (ordersEnemies + 1));
            for (var i = 0; i < enCount; i++)
            {
                var pos = transform.position + Vector3.up + Quaternion.Euler(0, i * 360f / enCount, 0) * new Vector3(0, 0, 10);
                var go = Instantiate(enemiesPrefabs[i % enemiesPrefabs.Length], pos, Quaternion.identity);
                var entity = go.GetComponent<Entity>();

                aliveEnemies.Add(entity);
                
                entity.GetModule<HealthEntityModule>().onDie.AddListener(() => OnEnemyDie(entity));
            }

            if (ordersEnemies == 0)
            {
                GameController.instance.player.GetModule<HealthEntityModule>().onDie.RemoveListener(OnPlayerDie);
                GameController.instance.player.GetModule<HealthEntityModule>().onDie.AddListener(OnPlayerDie);
            }

            ordersEnemies++;
        }

        public IEnumerator BellDissolve(bool dissolve)
        {
            var time = 0f;
            var startColor = !dissolve ? 1f : 0f;
            var endColor = dissolve ? 1f : 0f;

            if(!dissolve) foreach (var col in GetComponents<Collider>()) col.enabled = true;
            while (time < dissolvingTime)
            {
                time += Time.deltaTime;
                material.SetFloat(Dissolve, Mathf.Lerp(startColor, endColor, time / dissolvingTime));
                yield return null;
            }
            if(dissolve) foreach (var col in GetComponents<Collider>()) col.enabled = false;
        }
        
        public void OnEnemyDie(Entity e)
        {
            if (!aliveEnemies.Contains(e)) return;
            aliveEnemies.Remove(e);
            
            if(aliveEnemies.Count == 0)
            {
                StartCoroutine(_OnAllDied());
                StartCoroutine(BellDissolve(false));
                if(ordersEnemies >= ordersToWin)
                {
                    InGameHUD.instance.OpenSkill(skill);
                    ordersEnemies = 0;
                }
            }
        }

        private void OnPlayerDie()
        {
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