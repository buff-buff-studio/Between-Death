using System.Collections.Generic;
using System.Linq;
using Refactor.Data;
using Refactor.Interface.Widgets;
using UnityEngine;

namespace Refactor.Interface.Windows
{
    public class SettingsWindow : Window
    {
        public TabWidget tab;
        public Window restoreAlert;
        public Settings settings;

        public override void Open()
        {
            settings.Load();
            base.Open();
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
                    return true;
                }

                case InterfaceAction.MoveUp:
                {
                    if (canvas.GetCurrentWidget() == null) return true;
                    var widgets = (from Transform t in canvas.GetCurrentWidget().transform.parent select t.GetComponent<Widget>() into w where w != null select w).ToList();

                    var i = (widgets.IndexOf(canvas.GetCurrentWidget()) - 1);
                    if (i < 0) i = widgets.Count - 1;
                    canvas.SetCurrentWidget(widgets[i]);
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