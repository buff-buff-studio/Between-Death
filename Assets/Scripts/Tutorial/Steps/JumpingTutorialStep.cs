using UnityEngine;

namespace Refactor.Tutorial.Steps
{
    public class JumpingTutorialStep : DefaultTutorialStep
    {
        public GameObject button;
        public GameObject[] platforms;
        
        public override void OnBegin()
        {
            base.OnBegin();
            
            input.DisableAllInput();
            input.canMoveCamera = true;
            input.canMove = true;
            input.canSlow = true;
            input.canInteract = true;
            input.canJump = true;
            
            controller.ShowBindingDisplay("jump");
            button.SetActive(true);

            foreach (var plat in platforms)
                plat.SetActive(true);
            
            controller.ShowTargetMarker(button.transform.position, Color.green);
        }
        
        public override void OnEnd()
        {
            base.OnEnd();
            controller.ShowBindingDisplay("");
            input.DisableAllInput();
            
            foreach (var plat in platforms)
                plat.SetActive(false);
            
            button.SetActive(false);
            controller.CloseTargetMarker();
        }
    }
}