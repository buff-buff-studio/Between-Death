using System;
using Refactor.Interface;
using UnityEngine;
using UnityEngine.Serialization;

namespace Refactor.Tutorial.Steps
{
    public class WellDoneTutorialStep : DefaultTutorialStep
    {
        [FormerlySerializedAs("ingameCanvas")] public InGameCanvas inGameCanvas;
        public float time = 3f;

        public override void OnBegin()
        {
            base.OnBegin();
            input.DisableAllInput();
        }

        public override void OnEnd()
        {
            base.OnEnd();
        }
        
        public void Update()
        {
            if (!isCurrent) return;
            if ((time -= Time.deltaTime) <= 0)
                controller.NextStep();
        }
    }
}