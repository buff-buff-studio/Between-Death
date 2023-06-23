using System;
using Refactor.I18n;
using UnityEngine;

namespace Refactor.Data
{
    public class SettingsLoader : Singleton<SettingsLoader>
    {
        public Settings settings;

        protected override void OnEnable()
        {
            base.OnEnable();
            settings.Load();
            settings.variableLanguage.onChanged.AddListener(_OnChangedLanguageSelector);
            settings.variableGraphicsQuality.onChanged.AddListener(_OnChangedGraphicsQuality);
            
            _OnChangedLanguageSelector();
            _OnChangedGraphicsQuality();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
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