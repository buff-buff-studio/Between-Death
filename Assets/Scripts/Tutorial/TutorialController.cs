using System;
using System.Collections.Generic;
using Refactor.Interface;
using TMPro;
using UnityEngine;

namespace Refactor.Tutorial
{
    public class TutorialActions
    {
        public Action OnCompleted;
        public bool isCompleted;
        public bool isCurrentAction;
        public void Complete()
        {
            if(isCompleted) return;
            if(!isCurrentAction) return;
            OnCompleted?.Invoke();
            isCompleted = true;
            isCurrentAction = false;
        }

        public TutorialActions()
        {
            
        }
    }
    
    public class TutorialController : MonoBehaviour
    {

        private static TutorialController _instance;
        public static TutorialController Instance => _instance ? _instance : FindObjectOfType<TutorialController>(); 
        
        
        [Header("STEPS")]
        public TutorialStep[] steps;

        [Header("REFERENCE")] 
        public TMP_Text labelTutorialText;
        public ClosableCanvasGroup tutorialCanvasGroup;
        
        [Header("STATE")]
        public TutorialStep currentStep;

        public TutorialActions OnMove = new TutorialActions();
        public TutorialActions OnJump = new TutorialActions();
        public TutorialActions OnElementChanged = new TutorialActions();
        public TutorialActions OnDash = new TutorialActions();
        public TutorialActions OnCombo = new TutorialActions();
        private int index;
        

        private TutorialActions GetCurrentAction()
        {
            TutorialActions action = index switch
            {
                1 => OnMove,
                2 => OnJump,
                3 => OnElementChanged,
                4 => OnDash,
                5 => OnCombo,
                _ => new TutorialActions()
            };

            return action;
        }

        public void SetCurrent()
        {
            index++;
            GetCurrentAction().isCurrentAction = true;
        }
        private void OnEnable()
        {
            OnMove.OnCompleted += NextStep;
            OnJump.OnCompleted += NextStep;
            OnElementChanged.OnCompleted += NextStep;
            OnDash.OnCompleted += NextStep;
            OnCombo.OnCompleted += NextStep;
        }
        private void OnDisable()
        {
            OnMove.OnCompleted -= NextStep;
            OnJump.OnCompleted -= NextStep;
            OnElementChanged.OnCompleted += NextStep;
            OnDash.OnCompleted -= NextStep;
            OnCombo.OnCompleted -= NextStep;
        }

        private void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            
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