using System.Collections.Generic;
using Refactor.Data;
using Refactor.Entities;
using Refactor.Entities.Modules;
using Unity.Mathematics;
using UnityEngine;

namespace Refactor.Misc
{
    public interface IHealth
    {
        public float health { get; protected set; }
        public float maxHealth { get; protected set;}

        public bool canTakeDamage { get; set; }

        public float Damage(float amount)
        {
            if (health == 0 || !canTakeDamage)
                return 0;
            
            var a = math.min(health, amount);

            if (a > 0)
                ShowHealthDisplay(true, math.ceil(a));

            health = OnHealthChange(health - a);
            if(health <= 0 && a > 0) 
                OnDie();
            
            return a;
        }

        public float Heal(float amount)
        {
            var a = math.min(maxHealth - health, amount);
            
            if (a > 0)
                ShowHealthDisplay(false, math.ceil(a));

            health = OnHealthChange(health + a);
            
            return a;
        }

        public void RestoreLife()
        {
            health = maxHealth;
            OnHealthChange(health);
        }

        protected GameObject ShowHealthDisplay(bool isDamage, float amount)
        {
            return null;
        }

        protected float OnHealthChange(float newHealth)
        {
            return newHealth;
        }
        
        protected void OnDie() {}

        public GameObject GetGameObject();

        public Element GetElement();
    }

    public static class HealthHelper
    {
        private static readonly Collider[] _BufferColliders = new Collider[8];
        
        public static IEnumerable<IHealth> GetTargets(Vector3 position, float radius)
        {
            var healths = new List<IHealth>();
            var colliderCount = Physics.OverlapSphereNonAlloc(position, radius, _BufferColliders);
            
            for(var i = 0; i < colliderCount; i ++)
            {
                var col = _BufferColliders[i];
                var cmp = col.gameObject.GetComponent<HealthComponent>();
                if (cmp != null)
                {
                    if(!healths.Contains(cmp))
                        healths.Add(cmp);
                    
                    continue;
                }
                
                var e = col.gameObject.GetComponent<Entity>();
                if(e == null) continue;
                var mod = e.GetModule<HealthEntityModule>();
                if (mod == null) continue;
                if(!healths.Contains(mod))
                    healths.Add(mod);
            }

            return healths;
        }
    }
}