using System;
using DG.Tweening;
using Refactor.Data;
using Refactor.Entities;
using Refactor.Entities.Modules;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Refactor.Tutorial.Steps
{
    public class ComboTutorialStep : DefaultTutorialStep
    {
        public Entity player;
        private PlayerAttackEntityModule _module;
        public AttackCombo combo;

        public CanvasGroup displayGroup;
        public GameObject[] bindingDisplay;
        public GameObject[] textDisplay;

        public int comboIndex = 0;
        
        public override void OnBegin()
        {
            base.OnBegin();
            
            input.DisableAllInput();
            input.canMoveCamera = true;
            input.canMove = true;

            _module = player.GetModule<PlayerAttackEntityModule>();

            _module.onPlayerPerformAttack.AddListener(_OnPerformAttack);
            _module.onPlayerFailCombo.AddListener(_OnFailAttack);
            _module.onAttackEnd.AddListener(_OnAttacKEnd);

            _OnAttacKEnd();
            displayGroup.DOFade(1f, 0.5f);
        }

        private void _OnAttacKEnd()
        {
            input.canAttack0 = true;
            input.canAttack1 = false;
            
            comboIndex++;
            
            if(comboIndex > 4)
                controller.NextStep();
            
            textDisplay[0].SetActive(true);
            textDisplay[1].SetActive(false);
            
            bindingDisplay[0].SetActive(true);
            bindingDisplay[1].SetActive(false);
        }

        private void _OnFailAttack()
        {
            combo = null;
            _OnAttacKEnd();
        }
        
        private void _OnPerformAttack(AttackCombo currentCombo, int index)
        {
            if (index == -1)
            {
                combo = currentCombo;
                input.canAttack1 = comboIndex % 2 == 1;
                input.canAttack0 = comboIndex % 2 == 0;
            }
            else
            {
                _module.animator.speed = 1f;
                combo = null;
            }
        }

        public void Update()
        {
            if (!isCurrent) return;
            if (combo == null) return;
            var attack = combo.attacks[0];
            var half = (attack.nextAttackWindowStart + attack.nextAttackWindowEnd) / 2f;
            if (_module.time >= half)
            {
                _module.animator.speed = 0f;
                textDisplay[0].SetActive(false);
                textDisplay[1].SetActive(true);
                
                bindingDisplay[0].SetActive(comboIndex % 2 == 0);
                bindingDisplay[1].SetActive(comboIndex % 2 == 1);
            }
        }

        public override void OnEnd()
        {
            base.OnEnd();
            input.DisableAllInput();
            if (_module != null)
            {
                _module.onPlayerPerformAttack.RemoveListener(_OnPerformAttack);
                _module.onPlayerFailCombo.RemoveListener(_OnFailAttack);
                _module.onAttackEnd.RemoveListener(_OnAttacKEnd);
            }
            displayGroup.DOFade(0f, 0.5f);
        }
    }
}