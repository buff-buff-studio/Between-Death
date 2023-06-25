using System;
using Refactor.Entities;
using UnityEngine;

namespace Refactor.Tutorial.Steps
{
    public class WalkingTutorialStep : DefaultTutorialStep
    {
        public GameObject target;
        public Entity player;
        
        public override void OnBegin()
        {
            base.OnBegin();
            
            input.DisableAllInput();
            input.canMoveCamera = true;
            input.canMove = true;
            
            controller.ShowBindingDisplay("move");
            target.SetActive(true);
            
            controller.ShowTargetMarker(target.transform.position, Color.magenta);
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
            if (Vector3.Distance(player.transform.position, target.transform.position) < 1f)
                controller.NextStep();
        }
    }
}