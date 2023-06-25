using System.Collections;
using System.Collections.Generic;
using Refactor.Entities;
using Refactor.Entities.Modules;
using Refactor.Interface;
using Refactor.Misc;
using UnityEngine;

namespace Refactor.Tutorial.Steps
{
    public class DefeatTheEnemiesTutorialStep : DefaultTutorialStep
    {
        public Vector3 center = new Vector3(0, 0, 40);
        public Vector3 respawnPosition = new Vector3(0, 0, 30);
        public float radius = 5;
        public int count = 5;
        public Entity player;

        public List<Entity> aliveEnemies = new List<Entity>();

        public GameObject[] enemiesPrefabs;

        public override void OnBegin()
        {
            base.OnBegin();
            input.EnableAllInput();
            
            controller.ShowBindingDisplay("defeat_enemies");
            player.respawnPosition = respawnPosition;

            for (var i = 0; i < count; i++)
            {
                var pos = center + Quaternion.Euler(0, i * 360f / count, 0) * new Vector3(0, 0, radius);
                var go = Instantiate(enemiesPrefabs[i % enemiesPrefabs.Length], pos, Quaternion.identity);
                var entity = go.GetComponent<Entity>();
                var module = entity.GetModule<EnemyControllerEntityModule>();
                module.shouldAttack = true;
                module.attackTarget = player.transform;
                module.wanderingCenterPoint = center;
                module.NewTarget();
                aliveEnemies.Add(entity);
                
                entity.GetModule<HealthEntityModule>().onDie.AddListener(() => OnEnemyDie(entity));
            }
        }

        public override void OnEnd()
        {
            base.OnEnd();
            controller.ShowBindingDisplay("");
            input.DisableAllInput();
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
            controller.NextStep();
            yield return new WaitForSeconds(5f);
            var module = player.GetModule<HealthEntityModule>() as IHealth;
            module.Heal(module.maxHealth);
            controller.NextStep();
        }
    }
}