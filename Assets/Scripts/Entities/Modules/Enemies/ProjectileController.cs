using System.Collections.Generic;
using UnityEngine;

namespace Refactor.Entities.Modules
{
    public class ProjectileController : MonoBehaviour
    {
        private List<Projectile> pool = new List<Projectile>();
        [SerializeField]
        private Projectile prefab;
        
        public void CreateObject(Vector3 pos, Transform target)
        {
            Projectile obj = null;

            foreach (var g in pool)
            {
                if (!g.gameObject.activeInHierarchy)
                    obj = g;
            }
            
            if(obj == null)
            {
                obj = Instantiate(prefab);
                pool.Add(obj);
            }

            obj.transform.position = pos;
        }
    }
}