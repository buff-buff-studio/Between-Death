using DG.Tweening;
using Refactor.Misc;
using TMPro;
using UnityEngine;

namespace Refactor.Tutorial.Steps
{
    public class CameraTutorialStep : DefaultTutorialStep
    {
        public CanvasGroup rotatingAngleBox;
        public TMP_Text rotatingAngleLabel;
 
        public override void OnBegin()
        {
            base.OnBegin();
            input.DisableAllInput();
            input.canMoveCamera = true;

            controller.ShowBindingDisplay("move_camera");
            
            rotatingAngleBox.DOFade(1f, 0.5f);
            OrbitCamera.CAMERA_DELTA = 0;
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

            if (OrbitCamera.CAMERA_DELTA > 1_000f)
            {
                controller.NextStep();
                OrbitCamera.CAMERA_DELTA = 1_000f;
            }

            rotatingAngleLabel.text = $"{OrbitCamera.CAMERA_DELTA:F0}°";
        }
    }
}