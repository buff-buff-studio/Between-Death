using System;
using System.Collections.Generic;
using UnityEngine;

namespace Refactor.I18n
{
    public class LanguageManager
    {
        #region Constants
        public const string DEFAULT_LANGUAGE = "PtBR";
        #endregion

        #region Classes
        public class LanguageCache
        {
            public string path;
            public Dictionary<string, string> entries = new Dictionary<string, string>();
        
            public string Localize(string key)
            {
                return entries.GetValueOrDefault(key, $"?{key}?");
            }
        }
        #endregion

        #region Static Fields
        private static LanguageCache _cache;

        public static Action<LanguageCache> onLanguageChanged;

        public static LanguageCache cache
        {
            get 
            {
                if(_cache == null)
                    SetLanguage(DEFAULT_LANGUAGE);
                
                return _cache;
            }
        }
        
        public static string language
        {
            get => cache.path;
            set => SetLanguage(value);
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Localize a string using the current language
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Localize(string key)
        {
            return cache.Localize(key);
        }

        /// <summary>
        /// Set current language using language key
        /// </summary>
        /// <param name="language"></param>
        public static void SetLanguage(string language)
        {
            if(_cache != null && _cache.path == language)
                return;

            var file = Resources.Load<TextAsset>("Languages/" + language);
        
            var languageCache = new LanguageCache
            {
                path = language
            };

            var path = new string[8];

            foreach(string t in file.text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None))
            {
                if(t.Length == 0 || t[0] == '#') continue;

                var s = t.Replace("    ","\t");

                var idx = s.IndexOf('=');

                if(idx > 0)
                {
                    var key = s[..idx];
                    var value = s[(idx + 1)..];
                    
                    var depth = key.LastIndexOf('\t') + 1;

                    key = path[depth] = key.Trim();

                    if((value = value.Trim()).Length > 0)
                    {
                        value = value.Replace("\\n","\n");
                        languageCache.entries[string.Join('.', path, 0, depth + 1)] = value;
                    }
                }
                else
                {
                    int depth = s.LastIndexOf('\t') + 1;
                    path[depth] = s.Trim();
                }
            }

            _cache = languageCache;
            
            onLanguageChanged?.Invoke(_cache);
        }
        #endregion
    }
}