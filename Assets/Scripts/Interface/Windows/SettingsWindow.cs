using System;
using System.Collections.Generic;
using System.Linq;
using Refactor.Interface.Widgets;
using UnityEngine;

namespace Refactor.Interface.Windows
{
    public class SettingsWindow : Window
    {
        //public Widget[] firstWidget;
        public TabWidget tab;
        
        public override bool DoAction(InterfaceAction action)
        {
            switch (action)
            {
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

        public override Widget GetFirstWidget()
        {
            if(tab.readyForInput)
                return tab.tabFirstWidgets[tab.currentTab];
            return null;
        }
    }
}