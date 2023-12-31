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
        public CanvasGroup dimmer;

        [Header("EVENTS")] 
        public UnityEvent onConfirm;
        public UnityEvent onCancel;

        public override Widget GetFirstWidget()
        {
            return null;
        }

        public override void Open()
        {
            base.Open();
            var tweenId = $"window_{id}";
            dimmer.alpha = 0;
            dimmer.gameObject.SetActive(true);
            dimmer.DOFade(1f, 0.25f).SetId(tweenId);
            
            AudioSystem.PlaySound("ui_alert");
        }
        
        public override void Close()
        {
            AudioSystem.PlaySound("ui_click");
            base.Close();
            var tweenId = $"window_{id}";
            dimmer.DOFade(0f, 1f).SetId(tweenId)
                .OnKill(() =>
                {
                    dimmer.alpha = 0;
                    dimmer.gameObject.SetActive(false);
                }).OnComplete(() =>
                {
                    dimmer.alpha = 0;
                    dimmer.gameObject.SetActive(false);
                });
        }

        public override bool DoAction(InterfaceAction action)
        {
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