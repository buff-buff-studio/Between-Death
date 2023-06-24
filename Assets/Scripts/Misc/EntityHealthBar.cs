using System;
using DG.Tweening;
using Refactor.Entities;
using Refactor.Entities.Modules;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Refactor.Misc
{
    public class EntityHealthBar : MonoBehaviour
    {
        public Entity entity;
        private HealthEntityModule _module;
        public CanvasGroup canvasGroup;
        public Image healthBarImage;
        public RectTransform healthBar;
        private Transform _camera;
        
        private void OnEnable()
        {
            _camera = Camera.main!.transform;
            _module = entity.GetModule<HealthEntityModule>();
            _module.onHealthChange.AddListener(_OnChangeHealth);
            _OnChangeHealth((_module as IHealth).health);
            canvasGroup.alpha = 1f;
        }

        private void OnDisable()
        {
            _module.onHealthChange.RemoveListener(_OnChangeHealth);
        }

        private void Update()
        {
            var t = transform;
            var fw = _camera.transform.position - t.position;
            t.forward = -fw;
        }

        private void _OnChangeHealth(float health)
        {
            var iHealth = (_module as IHealth);
            
            var maxHealth = iHealth.maxHealth;

            if (health < maxHealth)
            {
                if(health == 0)
                    canvasGroup.DOFade(0f, 0.5f);

                healthBarImage.color = Color.yellow;
                healthBarImage.DOColor(Color.red, 0.5f);
                healthBar.DOSizeDelta(new Vector2(100 * health / maxHealth, 15), 0.5f);
            }
        }
    }
}