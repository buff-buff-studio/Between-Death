using UnityEngine;

namespace Refactor
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T instance => _GetInstance();
        private static T _instance; 
        
        private static T _GetInstance()
        {
            if (_instance != null) return _instance;
            
            _instance = FindObjectOfType<T>(true);
            
            if (_instance != null) return _instance;
            
            var go = new GameObject("");
            _instance = go.AddComponent<T>();
            go.name = $"Singleton:{_instance.GetType().Name}";

            return _instance;
        }

        protected virtual void OnEnable()
        {
            if (_instance != null)
            {
                if (_instance == this) return;
                Destroy(this);
                Debug.LogError($"The Singleton class {typeof(T).Name} must only have 1 instance per time!");
            }
            else
                _instance = (T) this;

            gameObject.name = $"Singleton:{GetType().Name}";
        }

        protected virtual void OnDisable()
        {
            if (_instance != null && _instance == this)
                _instance = null;
        }
    }
}