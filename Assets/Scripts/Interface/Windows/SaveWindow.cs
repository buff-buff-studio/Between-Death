using System;
using System.Collections;
using System.Collections.Generic;
using Refactor.Data;
using Refactor.I18n;
using Refactor.Interface.Widgets;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace Refactor.Interface.Windows
{
    public class SaveWindow : Window
    {
        public Save save;
        public Widget[] widgets;
        public TMP_Text[] saveInfo;
        public Window saveWindowAlert;

        protected override void OnEnable()
        {
            base.OnEnable();
            ReloadSaves();
        }

        public void ReloadSaves()
        {
            for (var i = 0; i < saveInfo.Length; i ++)
            {
                var data = save.GetSaveSnapshot(i);
                if (data.Exists)
                    saveInfo[i].text = data.CreationTime.ToString(LanguageManager.Localize("date_time_format"));
                else
                    saveInfo[i].text = LanguageManager.Localize("ui.saves.empty");
            }
        }
        
        public override Widget GetFirstWidget()
        {
            save.currentSlot = Mathf.Clamp(save.currentSlot, 0, widgets.Length);
            return widgets[save.currentSlot];
        }
        
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
                case InterfaceAction.Confirm:
                {
                    var w = canvas.GetCurrentWidget();
                    var n = w == null ? -1 : Array.IndexOf(widgets, w);
                    if (n != -1)
                    {
                        var data = save.GetSaveSnapshot(n);
                        if (data.Exists)
                        {
                            save.LoadFrom(n);
                        }
                        else
                        {
                            save.ResetData();
                            save.SaveTo(n);
                        }
               
                        Close();
                        canvas.StartCoroutine(_LoadScene("Scenes/Game_Playground"));
                    }

                    return true;
                }
                case InterfaceAction.ActionThird:
                {
                    var w = canvas.GetCurrentWidget();
                    var n = w == null ? -1 : Array.IndexOf(widgets, w);
                    if (n != -1)
                    {
                        save.currentSlot = n;
                        saveWindowAlert.Open();
                    }

                    return true;
                }
                case InterfaceAction.Cancel:
                {
                    if (canClose)
                    {
                        if(openOnClose)
                            canvas.CloseThenOpen(this, openOnClose);
                        else
                            Close();
                    }
                    return true;
                }
                default:
                    return false;
            }
        }
        
        private IEnumerator _LoadScene(string scene)
        {
            yield return new WaitForSeconds(1f);
            LoadingScreen.LoadScene(scene);
        }

        public override IEnumerable<string> GetBindingActions()
        {
            var w = canvas.GetCurrentWidget();
            var n = w == null ? -1 : Array.IndexOf(widgets, w);
            if (n != -1)
            {
                var data = save.GetSaveSnapshot(n);
                if (data.Exists)
                    yield return "reset";
                yield return "confirm";
            }
            
            yield return "move_y";
            yield return "back";
        }

        public void SetSelectSlot(int slot)
        {
            canvas.SetCurrentWidget(widgets[slot]);
        }
    }
}