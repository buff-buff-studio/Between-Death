using System.Collections;
using UnityEngine;

namespace Refactor.Tutorial.Steps
{
    public class StartingTutorialStep : TutorialStep
    {
        public override void OnBegin()
        {
            base.OnBegin();
            input.canInput = false;
            StartCoroutine(_Wait());
        }

        public override void OnEnd()
        {
            base.OnEnd();
            input.canInput = true;
            input.DisableAllInput();
        }

        private IEnumerator _Wait()
        {
            yield return new WaitForSeconds(0.5f);
            controller.NextStep();
        }
    }
}