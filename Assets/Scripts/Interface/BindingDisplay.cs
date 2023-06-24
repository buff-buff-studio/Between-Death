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

            public Sprite GetSprite(GameInput.ControlScheme scheme)
            {
                return scheme switch
                {
                    GameInput.ControlScheme.Xbox => xbox,
                    GameInput.ControlScheme.Playstation => playstation,
                    GameInput.ControlScheme.Gamepad => gamepad,
                    _ => desktop
                };
            }
        }

        public Image displayImage;
        public TMP_Text displayName;
        
        public SpritePalette spritePalette;
        public string actionDisplayName;
        private void OnEnable()
        {
            LanguageManager.onLanguageChanged += _ReloadLabel;

            _ReloadSprite();
            _ReloadLabel(null);
            GameInput.OnChangeControlScheme += _OnChangeControlScheme;
        }

        private void OnDisable()
        {
            LanguageManager.onLanguageChanged -= _ReloadLabel;
            GameInput.OnChangeControlScheme -= _OnChangeControlScheme;
        }

        private void _ReloadSprite()
        {
            displayImage.sprite = spritePalette.GetSprite(GameInput.CurrentControlScheme);
        }

        private void _OnChangeControlScheme()
        {
            _ReloadSprite();
        }

        private void _ReloadLabel(LanguageManager.LanguageCache cache)
        {
            if (displayName == null) return;
            displayName.text = LanguageManager.Localize(actionDisplayName);
        }
    }
}