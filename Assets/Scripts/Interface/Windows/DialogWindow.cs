using System;
using System.Collections.Generic;
using DG.Tweening;
using Refactor.Audio;
using Refactor.Interface.Widgets;
using UnityEngine;
using UnityEngine.Events;

namespace Refactor.Interface.Windows
{
    public class DialogWindow : Window
    {
        [Header("REFERENCES")]

        public Widget[] widgets;

        [Header("EVENTS")] 
        public UnityEvent onConfirm;
        public UnityEvent onCancel;

        public override Widget GetFirstWidget()
        {
            return null;
        }

        public override void Open()
        {
            Debug.Log($"DialogWindow.Open()");
            base.Open();
            var tweenId = $"window_{id}";
            
            AudioSystem.PlaySound("ui_alert");
        }
        
        public override void Close()
        {
            Debug.Log($"DialogWindow.Close()");
            AudioSystem.PlaySound("ui_click");
            base.Close();
            var tweenId = $"window_{id}";
        }

        public override bool DoAction(InterfaceAction action)
        {
            Debug.Log($"DialogWindow.DoAction({action})");
            switch (action)
            {
                case InterfaceAction.Confirm:
                {
                    onConfirm.Invoke();
                    return true;
                }
                case InterfaceAction.Cancel:
                {
                    onCancel.Invoke();
                    return true;
                }
                default:
                    return false;
            }
        }

        public override IEnumerable<string> GetBindingActions()
        {
            yield break;
        }
    }
}