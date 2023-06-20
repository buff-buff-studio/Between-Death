using System;
using Refactor.Data;
using Refactor.I18n;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;
using UnityEngine.XR;
using InputDevice = UnityEngine.InputSystem.InputDevice;

namespace Refactor.Interface
{
    public class BindingDisplay : MonoBehaviour
    {
        public Image displayImage;
        public TMP_Text displayName;
        
        public BindingDisplayPalette palette;
        public InputActionAsset actions;
        public string actionName = "Fire";
        public string actionDisplayName;
        private void OnEnable()
        {
            LanguageManager.onLanguageChanged += _ReloadLabel;

            _ReloadSprite();
            _ReloadLabel(null);
        }

        private void OnDisable()
        {
            LanguageManager.onLanguageChanged -= _ReloadLabel;
        }

        private void _ReloadSprite()
        {
            if (displayImage == null) return;
            
            var action = actions.FindAction(actionName);
            var v = action.GetBindingDisplayString(InputBinding.MaskByGroup("Gamepad"));
            
            displayImage.sprite = palette.ResolveSprite(v);
        }

        private void _ReloadLabel(LanguageManager.LanguageCache cache)
        {
            if (displayName == null) return;
            displayName.text = LanguageManager.Localize(actionDisplayName);
        }
    }
}