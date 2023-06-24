using System;
using System.Collections;
using DG.Tweening;
using Refactor.I18n;
using Refactor.Interface;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Refactor.Tutorial
{
    public class NewTutorialController : Singleton<NewTutorialController>
    {
        [Serializable]
        public class BindingDisplayItem
        {
            public string name;
            public CanvasGroup display;
        }
        
        public BindingDisplayItem[] bindingDisplayItems;
        
        public TutorialStep[] steps;
        public int currentStepIndex = -1;

        public UnityEvent onTutorialBegin;
        public UnityEvent onTutorialEnd;

        public CanvasGroup tutorialBox;
        public TMP_Text tutorialBoxTitle;
        public TMP_Text tutorialBoxContent;
        public IngameGameInput input;

        public TutorialStep currentStep =>
            currentStepIndex >= 0 && currentStepIndex < steps.Length ? steps[currentStepIndex] : null;

        private void Awake()
        {
            steps = GetComponentsInChildren<TutorialStep>();
            foreach (var step in steps)
                step.controller = this;
            NextStep();
        }

        public void NextStep()
        {
            StartCoroutine(_NextStep());
        }

        private IEnumerator _NextStep()
        {
            var current = currentStep;

            if (current != null)
            {
                current.OnEnd();
                CloseTutorialBox();
                yield return new WaitForSeconds(1F);
            }
            
            currentStepIndex++;
            current = currentStep;

            if (current != null)
                current.OnBegin();
        }

        private void CloseTutorialBox()
        {
            tutorialBox.DOFade(0f, 0.5f);
        }

        public void ShowTutorialBox(string title, string content)
        {
            if (tutorialBox.alpha < 0.1f)
            {
                tutorialBoxTitle.text = LanguageManager.Localize(title);
                tutorialBoxContent.text = LanguageManager.Localize(content);
                tutorialBox.DOFade(1f, 0.5f);
            }
            else
            {
                StartCoroutine(_ShowTutorialBox(title, content));
            }
        }

        private IEnumerator _ShowTutorialBox(string title, string content)
        {
            tutorialBox.DOFade(0f, 0.25f);
            yield return new WaitForSeconds(0.25f);
            tutorialBoxTitle.text = LanguageManager.Localize(title);
            tutorialBoxContent.text = LanguageManager.Localize(content);
            tutorialBox.DOFade(1f, 0.25f);
        }

        public void ShowBindingDisplay(string binding)
        {
            foreach (var v in bindingDisplayItems)
            {
                if (v.name == binding)
                {
                    v.display.gameObject.SetActive(true);
                    v.display.DOFade(1f, 0.5f);
                }
                else
                {
                    if(v.display.gameObject.activeInHierarchy)
                        v.display.DOFade(0f, 0.5f).OnComplete(() => v.display.gameObject.SetActive(false));
                }
            }
        }
    }
}