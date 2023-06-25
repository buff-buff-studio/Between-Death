using System;
using Refactor.Interface;
using UnityEngine;

namespace Refactor.Tutorial.Steps
{
    public class WellDoneTutorialStep : DefaultTutorialStep
    {
        public IngameCanvas ingameCanvas;
        public float time = 3f;

        public override void OnBegin()
        {
            base.OnBegin();
            input.DisableAllInput();
        }

        public override void OnEnd()
        {
            base.OnEnd();
            ingameCanvas.QuitGame();
        }
        
        public void Update()
        {
            if (!isCurrent) return;
            if ((time -= Time.deltaTime) <= 0)
                controller.NextStep();
        }
    }
}