using System;
using Refactor.Interface;
using TMPro;
using UnityEngine;

namespace Refactor.Tutorial
{
    public class TutorialController : MonoBehaviour
    {
        [Header("STEPS")]
        public TutorialStep[] steps;

        [Header("REFERENCE")] 
        public TMP_Text labelTutorialText;
        public ClosableCanvasGroup tutorialCanvasGroup;
        
        [Header("STATE")]
        public TutorialStep currentStep;

        private void Awake()
        {
            NextStep();
        }

        public void NextStep()
        {
            var nextIndex = 0;
            if (currentStep != null)
            {
                currentStep.OnEnd();
                nextIndex = Array.IndexOf(steps, currentStep) + 1;
            }
            else
                TutorialHasBegun();

            if (steps == null || nextIndex >= steps.Length)
                OnEnd();
            else
            {
                currentStep = steps[nextIndex];
                currentStep.tutorialController = this;
                currentStep.OnBegin();
            }
        }

        public void TutorialHasBegun()
        {
            Debug.Log("[Tutorial] Tutorial has begun!");
        }

        public void OnEnd()
        {
            Debug.Log("[Tutorial] Tutorial has ended!");
        }

        public void SetQuestDisplay(string quest)
        {
            if (string.IsNullOrEmpty(quest))
            {
                CloseQuestDisplay();
                return;
            }
            
            labelTutorialText.text = quest;
            tutorialCanvasGroup.SetOpen(true);
        }

        public void CloseQuestDisplay()
        {
            tutorialCanvasGroup.SetOpen(false);
        }
    }
}