using System;
using Refactor.I18n;
using UnityEngine;

namespace Refactor.Data
{
    public class SettingsLoader : MonoBehaviour
    {
        public Settings settings;

        public void OnEnable()
        {
            settings.Load();
            settings.variableLanguage.onChanged.AddListener(_OnChangedLanguageSelector);
            settings.variableGraphicsQuality.onChanged.AddListener(_OnChangedGraphicsQuality);
            
            _OnChangedLanguageSelector();
            _OnChangedGraphicsQuality();
        }

        private void OnDisable()
        {
            settings.variableLanguage.onChanged.RemoveListener(_OnChangedLanguageSelector);
            settings.variableGraphicsQuality.onChanged.RemoveListener(_OnChangedGraphicsQuality);
        }

        private void _OnChangedGraphicsQuality()
        {
            var lv = (float)settings.variableGraphicsQuality.GetValue();
            QualitySettings.SetQualityLevel(Mathf.RoundToInt(lv));
        }
        
        private void _OnChangedLanguageSelector()
        {
            LanguageManager.SetLanguage(settings.languages[(int) settings.variableLanguage.GetValue()].id);
        }
    }
}