using Refactor.Data;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable InconsistentNaming
namespace Refactor.Misc
{
    public class HealthComponent : MonoBehaviour, IHealth
    {
        [Header("REFERENCES")]
        public GameObject prefabHealthIndicator;
        
        [Header("STATE")]
        [SerializeField]
        private float _health = 5;
        [SerializeField]
        private float _maxHealth = 10;
        public Element element = Element.None;
        
        [Header("EVENTS")]
        public UnityEvent onDie;
        public UnityEvent<float> onChangeHealth;
        
        float IHealth.health
        {
            get => _health;
            set { _health = value; onChangeHealth.Invoke(_health);}
        }

        float IHealth.maxHealth
        {
            get => _maxHealth;
            set => _maxHealth = value;
        }
        
        public GameObject GetGameObject()
        {
            return gameObject;
        }
        
        void IHealth.OnDie()
        {
            onDie?.Invoke();
        }
        
        GameObject IHealth.ShowHealthDisplay(bool isDamage, float amount)
        {
            if (prefabHealthIndicator == null) return null;
            
            amount = math.ceil(amount);
            var go = Object.Instantiate(prefabHealthIndicator, transform.position + Vector3.up, Quaternion.identity);
            go.GetComponent<DamageIndicator>().SetValue(isDamage, amount);

            return go;
        }
        
        public Element GetElement()
        {
            return element;
        }
    }
}