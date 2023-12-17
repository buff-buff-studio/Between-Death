using System.Collections;
using Refactor.Audio;
using Refactor.Interface.Widgets;
using UnityEngine;

namespace Refactor.Interface.Windows
{
    public class CreditsWindow : Window
    {
        public TabWidget tab;
        public override void Open()
        {
            base.Open();
        }

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
                    return base.DoAction(action);
            }
        }
    }
}