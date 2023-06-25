using System;
using System.Collections;
using DG.Tweening;
using Refactor.I18n;
using Refactor.Interface;
using TMPro;
using UnityEngine;

namespace Refactor.Tutorial
{
    public class TutorialController : Singleton<TutorialController>
    {
        [Serializable]
        public class BindingDisplayItem
        {
            public string name;
            public CanvasGroup display;
        }
        
        public BindingDisplayItem[] bindingDisplayItems;
        public TutorialTargetMarker tutorialTargetMarker;
        public TutorialStep[] steps;
        public int currentStepIndex = -1;
        
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

        public void ShowTargetMarker(Vector3 point, Color color)
        {
            if (tutorialTargetMarker.canvasGroup.alpha < 0.1f)
            {
                tutorialTargetMarker.canvasGroup.DOFade(1f, 0.5f);
                tutorialTargetMarker.targetPos = point;
                tutorialTargetMarker.color = color;
            }
            else
            {
                StartCoroutine(_ShowTargetMarker(point, color));
            }
        }

        public void CloseTargetMarker()
        {
            tutorialTargetMarker.canvasGroup.DOFade(0f, 0.25f);
        }

        private IEnumerator _ShowTargetMarker(Vector3 point, Color color)
        {
            tutorialTargetMarker.canvasGroup.DOFade(0f, 0.25f);
            yield return new WaitForSeconds(0.25f);
            tutorialTargetMarker.targetPos = point;
            tutorialTargetMarker.color = color;
            tutorialTargetMarker.canvasGroup.DOFade(1f, 0.25f);
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