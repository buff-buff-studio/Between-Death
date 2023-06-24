using System;
using Refactor.Entities;
using Refactor.Entities.Modules;
using Refactor.Misc;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Refactor.Props
{
    public class InfoButton : Interactible
    {
        public Transform text;
        private Transform _camera;
        public GameObject[] prefabsEntity;
        public Entity player;

        protected override void OnEnable()
        {
            base.OnEnable();
            _camera = Camera.main!.transform;
        }

        public void Update()
        {
            var t = text.transform;
            var fw = _camera.transform.position - t.position;
            t.forward = -fw;
        }

        public void HealPlayer()
        {
            var hem = player.GetModule<HealthEntityModule>() as IHealth;
            hem.Heal(hem.maxHealth - hem.health);
        }

        public void DamagePlayer()
        {
            var hem = player.GetModule<HealthEntityModule>() as IHealth;
            if(hem.health > 1)
                hem.Damage(1);
        }

        public void SpawnEnemy()
        {
            var prefab = prefabsEntity[Random.Range(0, prefabsEntity.Length)];
            var v = Quaternion.Euler(0, Random.Range(0, 360), 0) * new Vector3(7, 4, 0);
            var p = Instantiate(prefab, v, Quaternion.identity);
        }
    }
}