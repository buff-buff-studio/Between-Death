using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Refactor.Misc
{
    public class Pool : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        private PersistentDictionary<int, List<Object>> _pool;
        [SerializeField, HideInInspector]
        private PersistentDictionary<int, int> _existent;

        public int limitPerType = 5;
        
        private void Awake()
        {
            _pool =  new PersistentDictionary<int, List<Object>>();
            _existent = new PersistentDictionary<int, int>();
        }
            
        /// <summary>
        /// Returns the count of the objects in the pool with the set id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetPoolCount(int id)
        {
            List<Object> table = _pool.Get(id);
            return table?.Count ?? 0;
        }
        
        /// <summary>
        /// Returns all objects inside the pool with the set id
        /// </summary>
        /// <param name="id"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable GetPoolObjects<T>(int id) where T : Object
        {
            List<Object> table = _pool.Get(id);
            if (table == null)
                return null;
            return table;
        }
        
        /// <summary>
        /// Gets a object from the pool only if its available
        /// </summary>
        /// <param name="id"></param>
        /// <param name="active"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetPoolObjectIfAvailable<T>(int id, bool active = true) where T : Object
        {
            List<Object> table = _pool.Get(id);
            if (table == null || table.Count == 0)
                return null;
            
            T eT = (T) table[0];
            table.RemoveAt(0);
            if(active) 
                eT.GameObject().SetActive(true);
            if (table.Count == 0)
                _pool.Remove(id, out _);
            return eT;
        }
        
        /// <summary>
        /// Instantiate new object using a prefab
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="active"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Instantiate<T>(ResourceSlot<T> prefab, bool active = true) where T : Object
        {
            return Instantiate(prefab.value, active);
        }
        
        /// <summary>
        /// Instantiate new object using a prefab
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="position"></param>
        /// <param name="parent"></param>
        /// <param name="active"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Instantiate<T>(ResourceSlot<T> prefab, Vector3 position, Transform parent = null, bool active = true) where T : Object
        {
            return Instantiate(prefab.value, position, parent, active);
        }
        
        /// <summary>
        /// Instantiate new object using a prefab
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="parent"></param>
        /// <param name="active"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Instantiate<T>(ResourceSlot<T> prefab, Vector3 position, Quaternion rotation, Transform parent = null, bool active = true) where T : Object
        {
            return Instantiate(prefab.value, position, rotation, parent, active);
        }
        
        /// <summary>
        /// Instantiate new object using a prefab
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="position"></param>
        /// <param name="parent"></param>
        /// <param name="active"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Instantiate<T>(T prefab, Vector3 position, Transform parent = null, bool active = true) where T : Object
        {
            T o = Instantiate(prefab, active);
            GameObject go = o.GameObject();
            go.transform.position = position;
            go.transform.parent = parent;
            return o;
        }
        
        /// <summary>
        /// Instantiate new object using a prefab
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="parent"></param>
        /// <param name="active"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null, bool active = true) where T : Object
        {
            T o = Instantiate(prefab, active);
            GameObject go = o.GameObject();
            go.transform.position = position;
            go.transform.rotation = rotation;
            go.transform.parent = parent;
            return o;
        }
        
        /// <summary>
        /// Instantiate new object using a prefab
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="active"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Instantiate<T>(T prefab, bool active = true) where T : Object
        {
            int id = GetId(prefab);
            
            List<Object> table = _pool.Get(id);
            if (table == null || table.Count == 0)
            {
                T t = Object.Instantiate(prefab);
                _existent.Set(GetId(t), id);
                return t;
            }
            
            T eT = (T) table[0];
            table.RemoveAt(0);
            if(active) 
                eT.GameObject().SetActive(true);

            if (table.Count == 0)
                _pool.Remove(id, out _);
            return eT;
        }
        
        /// <summary>
        /// Destroys an object putting it into the pool
        /// </summary>
        /// <param name="o"></param>
        public new void Destroy(Object o)
        {
            Destroy(o, _existent.Get(GetId(o)));
        }
        
        /// <summary>
        /// Destroys an object using a custom id putting it into the pool
        /// </summary>
        /// <param name="o"></param>
        /// <param name="id"></param>
        public void Destroy(Object o, int id)
        {
            if (id == 0)
            {
                Object.Destroy(o);
                return;
            }
            
            List<Object> table = _pool.Get(id);
            if (table == null)
            {
                table = new List<Object>();
                table.Add(o);
                _pool.Set(id, table);
            }
            else
            {
                if (table.Count >= limitPerType)
                {
                    Object.Destroy(o.GameObject());
                    return;
                }
                
                table.Add(o);
            }
            
            GameObject go = o.GameObject();
            go.SetActive(false);
            go.transform.parent = transform;
        }
        
        /// <summary>
        /// Gets the id of an object
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public int GetId(Object prefab)
        {
            return prefab.GetInstanceID();
        }
    
        /// <summary>
        /// Creates a new object then puts it into the pool
        /// </summary>
        /// <param name="prefab"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T CreateOnPool<T>(T prefab) where T : Object
        {
            int id = GetId(prefab);

            if (id == 0)
                return null;
            
            List<Object> table = _pool.Get(id);
            if (table == null)
            {
                T o = Object.Instantiate(prefab);
                _existent.Set(GetId(o), id);
                
                table = new List<Object>();
                table.Add(o);
                _pool.Set(id, table);
                
                GameObject go = o.GameObject();
                go.SetActive(false);
                go.transform.parent = transform;
                return o;
            }
            
            if (table.Count >= limitPerType) 
                return null;
            
            T o2 = Object.Instantiate(prefab);
            _existent.Set(GetId(o2), id);
            
            table.Add(o2);
            _pool.Set(id, table);

            GameObject go2 = o2.GameObject();
            go2.SetActive(false);
            go2.transform.parent = transform;
            return o2;
        }
    }
}