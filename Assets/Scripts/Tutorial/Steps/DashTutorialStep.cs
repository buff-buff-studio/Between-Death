using Refactor.Data;
using Refactor.Entities;
using Refactor.Entities.Modules;
using UnityEngine;

namespace Refactor.Tutorial.Steps
{
    public class DashTutorialStep : DefaultTutorialStep
    {
        public GameObject target;
        public Entity player;
        
        public override void OnBegin()
        {
            base.OnBegin();

            input.DisableAllInput();
            input.canJump = true;
            input.canDash = true;
            input.canMoveCamera = true;
            input.canMove = true;

            if (player.element == Element.Order)
            {
                player.isGrounded = true;
                IngameGameInput.InputChangeElement.SetValue(false);
                IngameGameInput.InputChangeElement.SetValue(true);
                var module = player.GetModule<PlayerControllerEntityModule>();
                module.Handle__ChangeElement();
                IngameGameInput.InputChangeElement.SetValue(false);
            }

            controller.ShowBindingDisplay("dash");
            target.SetActive(true);
            controller.ShowTargetMarker(target.transform.position, Color.magenta);
            
            //player.GetModule<PlayerAttackEntityModule>().attackRadius = 1.5f;
        }
        
        public override void OnEnd()
        {
            base.OnEnd();
            controller.ShowBindingDisplay("");
            input.DisableAllInput();
            
            target.SetActive(false);
            controller.CloseTargetMarker();
        }
        
        private void FixedUpdate()
        {
            if (!isCurrent) return;
            if (Vector3.Distance(player.transform.position, target.transform.position) < 3.5f)
                controller.NextStep();
        }
    }
}