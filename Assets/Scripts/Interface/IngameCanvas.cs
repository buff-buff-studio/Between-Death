using System;
using System.Collections;
using DG.Tweening;
using Refactor.Entities;
using Refactor.Entities.Modules;
using Refactor.Interface.Windows;
using Refactor.Misc;
using UnityEngine;
using UnityEngine.UI;

namespace Refactor.Interface
{
    public class IngameCanvas : Canvas
    {
        public Window quitDialogWindow;
        public Window documentDialogWindow;
        public IngameGameInput ingameGameInput;
        public Image[] bloodStains;
        public Entity player;
        private float lastHealth = 0;
        public RectTransform healthBarFill;

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

            UpdateHealthBar(hlt.maxHealth);
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
            documentDialogWindow.Open();
            ingameGameInput.canInput = false;
        }

        public void OnCloseDocumentWindow()
        {
            StartCoroutine(_OnCloseDocumentWindow());
        }
        
        private IEnumerator _OnCloseDocumentWindow()
        {
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
            healthBarFill.DOSizeDelta(new Vector2(260 * health / module.maxHealth, 20), 0.5f);
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