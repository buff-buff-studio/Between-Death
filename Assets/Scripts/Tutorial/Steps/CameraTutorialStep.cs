using System;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace Refactor.Tutorial.Steps
{
    public class CameraTutorialStep : DefaultTutorialStep
    {
        public CanvasGroup rotatingAngleBox;
        public TMP_Text rotatingAngleLabel;
        public float angle;
        
        public override void OnBegin()
        {
            base.OnBegin();
            input.DisableAllInput();
            input.canMoveCamera = true;

            controller.ShowBindingDisplay("move_camera");
            
            rotatingAngleBox.DOFade(1f, 0.5f);
        }

        public override void OnEnd()
        {
            base.OnEnd();
            controller.ShowBindingDisplay("");
            input.DisableAllInput();
            
            rotatingAngleBox.DOFade(0f, 0.5f);
        }

        private void FixedUpdate()
        {
            if (!isCurrent) return;

            angle += IngameGameInput.InputCamera.magnitude;

            if (angle > 150f)
            {
                controller.NextStep();
                angle = 150f;
            }

            rotatingAngleLabel.text = $"{angle:F0}°";
        }
    }
}