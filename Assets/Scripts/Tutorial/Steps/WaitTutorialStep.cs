using System;
using System.Collections;
using UnityEngine;

namespace Refactor.Tutorial.Steps
{
    public class WaitTutorialStep : TutorialStep
    {
        public float time = 1;

        private void OnEnable()
        {
            if (isCurrent)
                StartCoroutine(_Task());
        }

        public override void OnBegin()
        { 
            base.OnBegin();
            StartCoroutine(_Task());
        }

        private IEnumerator _Task()
        {
            yield return new WaitForSeconds(time);
            tutorialController.NextStep();
        }
    }
}