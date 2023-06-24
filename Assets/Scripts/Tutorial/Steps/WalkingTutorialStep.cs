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
            if (Vector3.Distance(player.transform.position, target.transform.position) < 1f)
                controller.NextStep();
        }
    }
}