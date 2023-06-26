using Refactor.Entities;
using Refactor.Entities.Modules;
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
            
            controller.ShowTargetMarker(target.transform.position, Color.magenta);

            player.GetModule<PlayerAttackEntityModule>().attackRadius = 8f;
        }
        
        public override void OnEnd()
        {
            base.OnEnd();
            controller.ShowBindingDisplay("");
            input.DisableAllInput();
            
            target.gameObject.SetActive(false);
            controller.CloseTargetMarker();
        }

        private void FixedUpdate()
        {
            if (!isCurrent) return;
            if (Vector3.Distance(player.transform.position, target.transform.position) < 2.5f)
                controller.NextStep();
        }
    }
}