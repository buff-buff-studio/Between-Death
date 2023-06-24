using Refactor.Entities;
using UnityEngine;

namespace Refactor.Tutorial.Steps
{
    public class GoToColiseumEntranceTutorialStep : DefaultTutorialStep
    {
        public GameObject target;
        public Entity player;
        
        public override void OnBegin()
        {
            base.OnBegin();
            
            input.DisableAllInput();
            input.canMoveCamera = true;
            input.canMove = true;
            input.canRun = true;
            input.canJump = true;

            controller.ShowBindingDisplay("");
            target.gameObject.SetActive(true);
        }
        
        public override void OnEnd()
        {
            base.OnEnd();
            controller.ShowBindingDisplay("");
            input.DisableAllInput();
            
            target.gameObject.SetActive(false);
        }

        private void FixedUpdate()
        {
            if (!isCurrent) return;
            if (Vector3.Distance(player.transform.position, target.transform.position) < 2.5f)
                controller.NextStep();
        }
    }
}