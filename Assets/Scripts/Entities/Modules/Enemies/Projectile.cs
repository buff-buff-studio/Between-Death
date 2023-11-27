using System;
using Refactor.Misc;
using UnityEngine;

namespace Refactor.Entities.Modules
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody body;
        [SerializeField]
        private float velocity;
        public Transform target;

        private void OnEnable()
        {
            Invoke(nameof(Deactivate), 5);
        }

        private void FixedUpdate()
        {
            body.velocity = (transform.forward * (velocity * Time.deltaTime));
            transform.LookAt(target);
        }

        private void Deactivate()
        {
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                ApplyDamageFor(1, 2);
                Deactivate();
            }
        }
        
        protected void ApplyDamageFor(float damage, float radius)
        {
            var p = transform.position;
            var pos = p + Vector3.up;/* + _controllerEntity.body.rotation * attackOffset;*/
            var fw = transform.forward;
            
            foreach (var damageTarget in HealthHelper.GetTargets(pos, radius))
            {
                if(damageTarget.GetGameObject().transform != target) continue;
                //if(target.GetGameObject() == entity.gameObject) continue;

                var delta = (damageTarget.GetGameObject().transform.position - p);
                var dir = delta.normalized;
                var dot = Vector3.Dot(fw, dir);
                if (dot < 0.1) continue;
                
                var hPos = (damageTarget.GetGameObject().transform.position + pos) / 2f;
                
                damageTarget.Damage(damage);
                
                if (damageTarget.health > 0 && damageTarget is HealthEntityModule module)
                {
                    var e = module.entity;
                    e.velocity = dir * 4f;
                }
                /*var go = Object.Instantiate(hitParticlesPrefab, hPos, Quaternion.identity);
                Object.Destroy(go, 2f);*/
            }
        }
    }
}