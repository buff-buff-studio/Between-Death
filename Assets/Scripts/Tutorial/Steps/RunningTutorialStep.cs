using System;
using DG.Tweening;
using Refactor.Entities;
using TMPro;
using UnityEngine;

namespace Refactor.Tutorial.Steps
{
    public class RunningTutorialStep : DefaultTutorialStep
    {
        public CanvasGroup runningDistanceBox;
        public TMP_Text runningDistanceLabel;
        public float distance;
        
        public Entity player;
        
        public override void OnBegin()
        {
            base.OnBegin();
            input.DisableAllInput();
            input.canMoveCamera = true;
            input.canMove = true;
            input.canSlow = true;

            controller.ShowBindingDisplay("run");

            runningDistanceBox.DOFade(1f, 0.5f);
        }
        
        public override void OnEnd()
        {
            base.OnEnd();
            controller.ShowBindingDisplay("");
            input.DisableAllInput();
            runningDistanceBox.DOFade(0f, 0.5f);
        }

        private void FixedUpdate()
        {
            if (!isCurrent) return;
            
            if (IngameGameInput.InputSlowing.value)
                distance += player.controller.velocity.magnitude * Time.fixedDeltaTime;

            runningDistanceLabel.text = $"{distance:F1}m";

            if (distance >= 15)
            {
                distance = 15;
                controller.NextStep();
            }
        }
    }
}