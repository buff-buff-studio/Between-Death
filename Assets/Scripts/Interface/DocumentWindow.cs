using System.Collections.Generic;
using DG.Tweening;
using Refactor.Interface.Widgets;
using Refactor.Interface.Windows;
using UnityEngine;

namespace Refactor.Interface
{
    public class DocumentWindow : Window
    {
        [Header("REFERENCES")]
        public CanvasGroup dimmer;
        
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
        }
        
        public override bool DoAction(InterfaceAction action)
        {
            switch (action)
            {
                case InterfaceAction.Cancel:
                {
                    Close();
                    return true;
                }
                default:
                    return false;
            }
        }
        
        public override void Close()
        {
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
        
        public override IEnumerable<string> GetBindingActions()
        {
            yield break;
        }
    }
}