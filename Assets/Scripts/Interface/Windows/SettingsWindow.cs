using System.Collections.Generic;
using System.Linq;
using Refactor.Audio;
using Refactor.Data;
using Refactor.Interface.Widgets;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Refactor.Interface.Windows
{
    public class SettingsWindow : Window
    {
        public TabWidget tab;
        public Window restoreAlert;
        public Settings settings;
        public TMP_Dropdown languageDropdown;

        [Header("EVENTS")]
        public UnityEvent onCancel;

        public override void Open()
        {
            settings.Load();
            base.Open();

            var options = settings.languages.Select(s => new TMP_Dropdown.OptionData(s.localizedName)).ToList();
            languageDropdown.ClearOptions();
            languageDropdown.AddOptions(options);
            languageDropdown.value = (int) settings.variableLanguage.GetValue();
        }

        public override void Close()
        {
            settings.Save();
            base.Close();
        }

        public override bool DoAction(InterfaceAction action)
        {
            switch (action)
            {
                case InterfaceAction.ActionThird:
                    restoreAlert.Open();
                    return true;
                
                case InterfaceAction.TriggerLeft:
                    tab.PrevTab();
                    return true;
                case InterfaceAction.TriggerRight:
                    tab.NextTab();
                    return true;

                case InterfaceAction.MoveDown:
                {
                    if (canvas.GetCurrentWidget() == null) return true;
                    var widgets = (from Transform t in canvas.GetCurrentWidget().transform.parent select t.GetComponent<Widget>() into w where w != null select w).ToList();

                    var i = (widgets.IndexOf(canvas.GetCurrentWidget()) + 1)%widgets.Count;
                    canvas.SetCurrentWidget(widgets[i]);
                    AudioSystem.PlaySound("ui_click");
                    return true;
                }

                case InterfaceAction.MoveUp:
                {
                    if (canvas.GetCurrentWidget() == null) return true;
                    var widgets = (from Transform t in canvas.GetCurrentWidget().transform.parent select t.GetComponent<Widget>() into w where w != null select w).ToList();

                    var i = (widgets.IndexOf(canvas.GetCurrentWidget()) - 1);
                    if (i < 0) i = widgets.Count - 1;
                    canvas.SetCurrentWidget(widgets[i]);
                    AudioSystem.PlaySound("ui_click");
                    return true;
                }

                case InterfaceAction.Cancel:
                {
                    onCancel.Invoke();
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
                    return base.DoAction(action);
            }
        }

        public override IEnumerable<string> GetBindingActions()
        {
            yield return "reset";
            yield return "move_y";
            
            if (canClose)
                yield return openOnClose == null ? "close" : "back";
        }

        public override Widget GetFirstWidget()
        {
            if(tab.readyForInput)
                return tab.tabFirstWidgets[tab.currentTab];
            return null;
        }
    }
}