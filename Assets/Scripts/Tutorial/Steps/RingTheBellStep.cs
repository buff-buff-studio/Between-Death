using UnityEngine;

namespace Refactor.Tutorial.Steps
{
    public class RingTheBellStep : DefaultTutorialStep
    {
        public override void OnBegin()
        {
            base.OnBegin();
            
            input.DisableAllInput();
            input.canMoveCamera = true;
            input.canMove = true;
            //input.canRun = true;
            input.canJump = true;
            input.canAttack0 = true;
            //input.canChangeElement = true;

            controller.ShowBindingDisplay("attackL");
        }
        
        public override void OnEnd()
        {
            base.OnEnd();
            controller.ShowBindingDisplay("");
            input.DisableAllInput();
        }
    }
}