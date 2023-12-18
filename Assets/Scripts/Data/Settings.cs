using System;
using System.IO;
using System.Text;
using Refactor.Data.Variables;
using Refactor.I18n;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Refactor.Data
{
    [CreateAssetMenu(menuName = "Refactor/Data/Settings", fileName = "Settings", order = 100)]
    public class Settings : ScriptableObject
    {
        [Serializable]
        public struct LanguageSnapshot
        {
            public string id;
            public string localizedName;
        }
        
        public Variable[] variables = Array.Empty<Variable>();

        public Variable variableLanguage;
        public Variable variableGraphicsQuality;
        public LanguageSnapshot[] languages;
        
        public void OnEnable()
        {
            Load();
        }

        public void Save()
        {
            var filePath = Application.persistentDataPath + "/settings.dat";
            
            var builder = new StringBuilder();

            foreach(var v in variables)
            {
                builder.AppendLine(v.ToJson());
            }

            File.WriteAllText(filePath, builder.ToString());
            Debug.Log($"[!] Saved settings to '{filePath}'");
        }

        public void Reset()
        {
            var filePath = Application.persistentDataPath + "/settings.dat";

            if(File.Exists(filePath))
                File.Delete(filePath);

            Load();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void Load()
        {
            var filePath = Application.persistentDataPath + "/settings.dat";

            if(!File.Exists(filePath))
            {
                foreach(var v in variables)
                {
                    v.Reset();
                    v.ForceUpdate();
                }
                Save();

                return;
            }

            var lines = File.ReadAllLines(filePath);

            for(var i = 0; i < lines.Length; i ++)
            {
                variables[i].LoadFromJson(lines[i]);
                //JsonUtility.FromJsonOverwrite(lines[i], variables[i]);
                variables[i].ForceUpdate();
            }
            
            Debug.Log($"[!] Loaded settings from '{filePath}'");
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(Settings))]
    public class SettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var settings = (target as Settings)!;

            base.OnInspectorGUI();

            if(GUILayout.Button("Save"))
                settings.Save();

            if(GUILayout.Button("Load"))
                settings.Load();

            if(GUILayout.Button("Reset"))
                settings.Reset();
        }
    }
    #endif
}