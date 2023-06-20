using System;
using Refactor.Data;
using Refactor.Data.Variables;
using Refactor.Misc;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

// ReSharper disable InconsistentNaming
namespace Refactor.Entities.Modules
{
    [Serializable]
    public class HealthEntityModule : EntityModule, IHealth
    {
        [Header("REFERENCES")]
        public GameObject prefabHealthIndicator;

        [Header("STATE")] 
        [SerializeField] 
        private Value<float> _health = 0;
        [SerializeField]
        private Value<float> _maxHealth = 10;
        
        [Header("EVENTS")]
        public UnityEvent onDie;
        public UnityEvent<float> onHealthChange;

        float IHealth.health
        {
            get => _health.value;
            set => _health.value = value;
        }

        float IHealth.maxHealth
        {
            get => _maxHealth.value;
            set => _maxHealth.value = value;
        }
        
        public GameObject GetGameObject()
        {
            return entity.gameObject;
        }
        
        void IHealth.OnDie()
        {
            onDie?.Invoke();
        }

        float IHealth.OnHealthChange(float newHealth)
        {
            onHealthChange.Invoke(newHealth);
            return newHealth;
        }

        GameObject IHealth.ShowHealthDisplay(bool isDamage, float amount)
        {
            if (prefabHealthIndicator == null) return null;

            amount = math.ceil(amount);
            var go = Object.Instantiate(prefabHealthIndicator, entity.transform.position + Vector3.up, Quaternion.identity);
            go.GetComponent<DamageIndicator>().SetValue(isDamage, amount);

            return go;
        }

        public Element GetElement()
        {
            return entity.element;
        }

        #region Module Behaviour
        public override Color GetColor()
        {
            return new Color(0.6f, 0f, 0f);
        }
        #endregion
    }
}