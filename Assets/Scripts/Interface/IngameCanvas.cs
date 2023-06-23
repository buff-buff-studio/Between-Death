using System.Collections;
using DG.Tweening;
using Refactor.Interface.Windows;
using UnityEngine;

namespace Refactor.Interface
{
    public class IngameCanvas : Canvas
    {
        public Window quitDialogWindow;
        public IngameGameInput ingameGameInput;
        
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
    }
}