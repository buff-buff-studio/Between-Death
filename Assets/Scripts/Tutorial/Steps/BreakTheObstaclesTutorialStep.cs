using System;
using UnityEngine;

namespace Refactor.Tutorial.Steps
{
    public class BreakTheObstaclesTutorialStep : DefaultTutorialStep
    {
        public GameObject[] obstacles;

        public bool canChangeElement = false;
        public GameObject[] activateOnBegin;

        public override void OnBegin()
        {
            base.OnBegin();
            
            input.DisableAllInput();
            input.canMoveCamera = true;
            input.canMove = true;
            input.canSlow = true;
            input.canJump = true;
            input.canAttack0 = true;
            input.canChangeElement = canChangeElement;

            controller.ShowBindingDisplay(canChangeElement ? "change_element" : "attackL");

            foreach (var o in obstacles)
                o.SetActive(true);

            foreach (var o in activateOnBegin)
                o.SetActive(true);
        }
        
        public override void OnEnd()
        {
            base.OnEnd();
            controller.ShowBindingDisplay("");
            input.DisableAllInput();
            
            foreach (var o in obstacles)
                o.SetActive(false);
        }
    }
}