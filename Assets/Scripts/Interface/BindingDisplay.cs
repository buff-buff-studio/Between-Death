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

            _ReloadSprite(GameInput.CurrentControlScheme);
            _ReloadLabel(null);
            GameInput.OnChangeControlScheme += _OnChangeControlScheme;
        }

        private void OnDisable()
        {
            LanguageManager.onLanguageChanged -= _ReloadLabel;
            GameInput.OnChangeControlScheme -= _OnChangeControlScheme;
        }

        private void _ReloadSprite(GameInput.ControlScheme scheme)
        {
            displayImage.sprite = scheme switch
            {
                GameInput.ControlScheme.Xbox => spritePalette.xbox,
                GameInput.ControlScheme.Playstation => spritePalette.playstation,
                GameInput.ControlScheme.Gamepad => spritePalette.gamepad,
                _ => spritePalette.desktop
            };
        }

        private void _OnChangeControlScheme(GameInput.ControlScheme scheme)
        {
            _ReloadSprite(scheme);
        }

        private void _ReloadLabel(LanguageManager.LanguageCache cache)
        {
            if (displayName == null) return;
            displayName.text = LanguageManager.Localize(actionDisplayName);
        }
    }
}