using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Refactor.Audio
{
    [CreateAssetMenu(menuName = "Refactor/AudioPalette", fileName = "AudioPalette", order = 20)]
    public class AudioPalette : ScriptableObject
    {
        #region Classes
        [Serializable]
        public class AudioEntry
        {
            public string key;
            public Audio[] audios;
        }

        [Serializable]
        public class Audio
        {
            public AudioClip clip;
            public float volume = 1f;
        }
        #endregion

        #region Fields
        public AudioEntry[] entries;
        [NonSerialized]
        private Dictionary<int, Audio[]> _cacheAudios;
        #endregion

        public Audio GetAudio(int key)
        {
            if(_cacheAudios == null)
            {
                _cacheAudios = new Dictionary<int, Audio[]>();

                foreach(AudioEntry entry in entries)
                {
                    _cacheAudios[AudioSystem.HashString(entry.key)] = entry.audios; 
                }
            }

            Audio[] audios = _cacheAudios.GetValueOrDefault(key, null);

            if(audios == null || audios.Length == 0)
                return null;

            return audios[UnityEngine.Random.Range(0, audios.Length)];
        }

        public Audio GetAudio(string key)
        {
            return GetAudio(AudioSystem.HashString(key));
        }
    }

    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(AudioPalette.Audio), true)]
    public class VariableDrawer : PropertyDrawer
    {   
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            float width = position.width;

            var rectClp = new Rect(position.x, position.y, width * 2/3f - 2, position.height);
            var rectVol = new Rect(position.x + width * 2/3f, position.y, width/3f, position.height);

            EditorGUI.PropertyField(rectClp, property.FindPropertyRelative("clip"), GUIContent.none);
            EditorGUI.PropertyField(rectVol, property.FindPropertyRelative("volume"), GUIContent.none, true);        

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
    #endif
}