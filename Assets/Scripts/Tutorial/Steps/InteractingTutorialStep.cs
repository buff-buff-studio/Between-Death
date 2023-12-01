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
            input.canSlow = true;
            input.canInteract = true;
            
            controller.ShowBindingDisplay("interact");
            button.gameObject.SetActive(true);
            
            controller.ShowTargetMarker(button.transform.position, Color.green);
        }
        
        public override void OnEnd()
        {
            Debug.Log("ENDED");
            base.OnEnd();
            controller.ShowBindingDisplay("");
            input.DisableAllInput();
            
            button.gameObject.SetActive(false);
            controller.CloseTargetMarker();
        }
    }
}