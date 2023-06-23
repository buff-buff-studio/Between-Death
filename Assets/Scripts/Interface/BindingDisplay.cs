using System;
using System.Security.Cryptography;
using Refactor.Data;
using Refactor.I18n;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;
using UnityEngine.XR;
using InputDevice = UnityEngine.InputSystem.InputDevice;

namespace Refactor.Interface
{
    
    public class BindingDisplay : MonoBehaviour
    {
        [Serializable]
        public struct SpritePalette
        {
            public Sprite desktop;
            public Sprite gamepad;
            public Sprite xbox;
            public Sprite playstation;
        }

        public Image displayImage;
        public TMP_Text displayName;
        
        public SpritePalette spritePalette;
        public string actionDisplayName;
        private void OnEnable()
        {
            LanguageManager.onLanguageChanged += _ReloadLabel;

            _ReloadSprite(CanvasInput.controlScheme);
            _ReloadLabel(null);
            CanvasInput.OnChangeControlScheme += _OnChangeControlScheme;
        }

        private void OnDisable()
        {
            LanguageManager.onLanguageChanged -= _ReloadLabel;
            CanvasInput.OnChangeControlScheme -= _OnChangeControlScheme;
        }

        private void _ReloadSprite(CanvasInput.ControlScheme scheme)
        {
            displayImage.sprite = scheme switch
            {
                CanvasInput.ControlScheme.Xbox => spritePalette.xbox,
                CanvasInput.ControlScheme.Playstation => spritePalette.playstation,
                CanvasInput.ControlScheme.Gamepad => spritePalette.gamepad,
                _ => spritePalette.desktop
            };
        }

        private void _OnChangeControlScheme(CanvasInput.ControlScheme scheme)
        {
            _ReloadSprite(scheme);
        }
        
        /*
        private void _ReloadSprite()
        {
            if (displayImage == null) return;
            
            var action = actions.FindAction(actionName);
            var v = action.GetBindingDisplayString(InputBinding.MaskByGroup("Gamepad"));
            
            displayImage.sprite = palette.ResolveSprite(v);
        }
        */

        private void _ReloadLabel(LanguageManager.LanguageCache cache)
        {
            if (displayName == null) return;
            displayName.text = LanguageManager.Localize(actionDisplayName);
        }
    }
}