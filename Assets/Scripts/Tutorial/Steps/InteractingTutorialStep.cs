using UnityEngine;

namespace Refactor.Tutorial.Steps
{
    public class InteractingTutorialStep : DefaultTutorialStep
    {
        public GameObject button;
        
        public override void OnBegin()
        {
            base.OnBegin();
            
            input.DisableAllInput();
            input.canMoveCamera = true;
            input.canMove = true;
            input.canRun = true;
            input.canInteract = true;
            
            controller.ShowBindingDisplay("interact");
            button.gameObject.SetActive(true);
        }
        
        public override void OnEnd()
        {
            base.OnEnd();
            controller.ShowBindingDisplay("");
            input.DisableAllInput();
            
            button.gameObject.SetActive(false);
        }
    }
}