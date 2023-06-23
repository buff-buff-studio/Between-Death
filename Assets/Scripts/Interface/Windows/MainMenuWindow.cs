using System;
using System.Collections;
using System.Collections.Generic;
using Refactor.Interface.Widgets;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Refactor.Interface.Windows
{
    public class MainMenuWindow : Window
    {
        [Header("REFERENCES")] 
        public Window windowSettings;
        public Window windowCredits;
        public Window windowSave;
        public Widget[] widgets;
        public TMP_Text labelController;

        public void PlayGame()
        {
            Close();
            canvas.CloseThenOpen(this, windowSave);
            //canvas.StartCoroutine(_LoadScene("Scenes/Game_Playground"));
        }
        
        public void PlayTutorial()
        {
            Close();
            canvas.StartCoroutine(_LoadScene("Scenes/Tutorial"));
        }

        public void Exit()
        {
            Close();
            canvas.StartCoroutine(_Quit());
        }

        private IEnumerator _LoadScene(string scene)
        {
            yield return new WaitForSeconds(1f);
            LoadingScreen.LoadScene(scene);
        }

        private IEnumerator _Quit()
        {
            yield return new WaitForSeconds(0.75f);
            Application.Quit();
            #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            #endif
        }
        
        public void OpenSettings()
        {
            canvas.CloseThenOpen(this, windowSettings);
        }
        
        public void OpenCredits()
        {
            canvas.CloseThenOpen(this, windowCredits);
        }

        protected override void Update()
        {
            base.Update();
            labelController.gameObject.SetActive(GameInput.CurrentControlScheme == GameInput.ControlScheme.Desktop);
        }

        public override Widget GetFirstWidget() => widgets[0];

        public override bool DoAction(InterfaceAction action)
        {
            switch (action)
            {
                case InterfaceAction.MoveDown:
                {
                    var index = Array.IndexOf(widgets, canvas.GetCurrentWidget());
                    if (index == -1) return false;

                    index = (index + 1) % widgets.Length;
                    canvas.SetCurrentWidget(widgets[index]);
                    return true;
                }
                case InterfaceAction.MoveUp:
                {
                    var index = Array.IndexOf(widgets, canvas.GetCurrentWidget());
                    if (index == -1) return false;

                    index = (index > 0) ? index - 1 : widgets.Length - 1;
                    canvas.SetCurrentWidget(widgets[index]);
                    return true;
                }
                default:
                    return false;
            }
        }

        public override IEnumerable<string> GetBindingActions()
        {
            yield return "move_y";
        }
    }
}