using System;
using DG.Tweening;
using Refactor.Entities;
using Refactor.Entities.Modules;
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
        private Transform _camera;
        private float lastHealth;
        private Color barColor;

        private void Awake()
        {
            barColor = healthBarImage.color;
        }

        private void OnEnable()
        {
            _camera = Camera.main!.transform;
            _module = entity.GetModule<HealthEntityModule>();
            _module.onHealthChange.AddListener(_OnChangeHealth);
            var hm = (_module as IHealth);
            _OnChangeHealth(hm.health);
            lastHealth = hm.health;
            if(canvasGroup != null)
                canvasGroup.alpha = Math.Abs(hm.health - hm.maxHealth) < 0.01f ? 0 : 1;
        }

        private void OnDisable()
        {
            _module.onHealthChange.RemoveListener(_OnChangeHealth);
        }

        private void Update()
        {
            if(canvasGroup == null) return;

            var t = transform;
            var fw = _camera.transform.position - t.position;
            t.forward = -fw;
        }

        private void _OnChangeHealth(float health)
        {
            var iHealth = (_module as IHealth);
            
            var maxHealth = iHealth.maxHealth;
            var color = lastHealth > health ? Color.yellow : Color.cyan;

            if (health < maxHealth)
            {
                //if(health == 0)

                if(canvasGroup != null)
                    canvasGroup.DOFade(health == 0 || Math.Abs(health - iHealth.maxHealth) < 0.01f ? 0f : 1f, 0.25f);
                //else
                    //canvasGroup.DOFade(1f, 0.5f);

                healthBarImage.color = color;
                healthBarImage.DOColor(barColor, 0.5f);
                healthBarImage.DOFillAmount(health / maxHealth, 0.5f);
            }
        }
    }
}