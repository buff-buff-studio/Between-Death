using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace Refactor.Tutorial.Steps
{
    public class CameraTutorialStep : DefaultTutorialStep
    {
        public float move = 0;
        
        public override void OnBegin()
        {
            base.OnBegin();
            input.DisableAllInput();
            input.canMoveCamera = true;

            controller.ShowBindingDisplay("move_camera");
        }

        public override void OnEnd()
        {
            base.OnEnd();
            controller.ShowBindingDisplay("");
            input.DisableAllInput();
        }

        private void FixedUpdate()
        {
            if (!isCurrent) return;

            move += IngameGameInput.InputCamera.magnitude;

            if (move > 100f)
                controller.NextStep();
        }
    }
}