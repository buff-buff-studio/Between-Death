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
        public RectTransform healthBar;
        private Transform _camera;
        
        private void OnEnable()
        {
            _camera = Camera.main!.transform;
            _module = entity.GetModule<HealthEntityModule>();
            _module.onHealthChange.AddListener(_OnChangeHealth);
            var hm = (_module as IHealth);
            _OnChangeHealth(hm.health);
            canvasGroup.alpha = Math.Abs(hm.health - hm.maxHealth) < 0.01f ? 0 : 1;
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
                //if(health == 0)
                    canvasGroup.DOFade(health == 0 || Math.Abs(health - iHealth.maxHealth) < 0.01f ? 0f : 1f, 0.25f);
                //else
                    //canvasGroup.DOFade(1f, 0.5f);

                healthBarImage.color = Color.yellow;
                healthBarImage.DOColor(Color.red, 0.5f);
                healthBar.DOSizeDelta(new Vector2(100 * health / maxHealth, 15), 0.5f);
            }
        }
    }
}