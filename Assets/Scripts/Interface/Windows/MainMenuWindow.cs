using System;
using System.Collections;
using System.Collections.Generic;
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
        public Widget[] widgets;

        public void PlayGame()
        {
            Close();
        }
        
        public void PlayTutorial()
        {
            Close();
        }

        public void Exit()
        {
            StartCoroutine(_Quit());
        }

        private IEnumerator _Quit()
        {
            Close();
            yield return new WaitForSeconds(0.25f);
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