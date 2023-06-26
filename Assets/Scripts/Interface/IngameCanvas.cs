using System;
using System.Collections;
using DG.Tweening;
using Refactor.Entities;
using Refactor.Entities.Modules;
using Refactor.Interface.Windows;
using Refactor.Data;
using Refactor.Misc;
using UnityEngine;
using UnityEngine.UI;

namespace Refactor.Interface
{
    public class IngameCanvas : Canvas
    {
        public Window quitDialogWindow;
        public Window documentDialogWindow;
        public CanvasGroup uiGroup;
        public IngameGameInput ingameGameInput;
        public Image[] bloodStains;
        public Entity player;
        private float lastHealth = 0;
        public Image healthBarFill;
        public Image elementIcon;
        public Sprite chaosIcon;
        public Sprite orderIcon;
        public InspectDoc documentWindow;

        private void OnEnable()
        {
            var module = player.GetModule<HealthEntityModule>();
            var hlt = (IHealth)module;
            lastHealth = hlt.health;
            module.onHealthChange.AddListener((h) =>
            {
                if (h < lastHealth)
                {
                    ShowDamageEffect();
                }

                lastHealth = h;
                UpdateHealthBar(h);
            });

            player.onChangeElement.AddListener(UpdateElementIcon);

            UpdateElementIcon();
            UpdateHealthBar(hlt.maxHealth);
        }
        
        public void UpdateElementIcon()
        {
            elementIcon.sprite = player.element switch
            {
                Element.Chaos => chaosIcon,
                Element.Order => orderIcon,
                _ => elementIcon.sprite
            };
        }

        public override void CallAction(InterfaceAction action)
        {
            if (action is InterfaceAction.Start && ingameGameInput.canInput)
            {
                quitDialogWindow.Open();
                ingameGameInput.canInput = false;
                return;
            }
            
            base.CallAction(action);
        }

        public void OpenDocumentWindow()
        {
            uiGroup.alpha = 0;
            documentDialogWindow.Open();
            ingameGameInput.canInput = false;
        }
        
        public void OpenDocumentWindow(DocumentText txt)
        {
            uiGroup.alpha = 0;
            documentWindow._documentData = txt;
            documentDialogWindow.Open();
            ingameGameInput.canInput = false;
        }

        public void CloseDocumentWindow()
        {
            StartCoroutine(_OnCloseDocumentWindow());
        }
        
        private IEnumerator _OnCloseDocumentWindow()
        {
            documentDialogWindow.Close();
            uiGroup.alpha = 1;
            yield return new WaitForSeconds(0.5f);
            ingameGameInput.canInput = true;
        }
        
        public void QuitGame()
        {
            quitDialogWindow.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
            StartCoroutine(_QuitGame());
        }

        private IEnumerator _QuitGame()
        {
            yield return new WaitForSeconds(1f);
            LoadingScreen.LoadScene("Scenes/Menu");
        }

        public void CloseQuitGame()
        {
            quitDialogWindow.Close();
            StartCoroutine(_CloseQuitGame());
        }
        
        private IEnumerator _CloseQuitGame()
        {
            yield return new WaitForSeconds(0.5f);
            ingameGameInput.canInput = true;
        }

        public void UpdateHealthBar(float health)
        {
            var module = (IHealth) player.GetModule<HealthEntityModule>();
            healthBarFill.DOFillAmount(health / module.maxHealth, 0.5f);
        }
        
        public void ShowDamageEffect()
        {
            foreach (var image in bloodStains)
            {
                image.DOColor(Color.white, 0.5f);
                image.DOColor(Color.clear, 0.5f).SetDelay(0.55f);
            }
        }
    }
}