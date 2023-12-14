using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "PlayerSpeechData", menuName = "RPG/PlayerSpeechData", order = 1)]
public class PlayerSpeechData : ScriptableObject
{
    public List<Speech> speeches;

    public Speech GetSpeech(string tag)
    {
        return speeches.Find(speech => string.Equals(speech.tag, tag, StringComparison.CurrentCultureIgnoreCase));
    }

    public Speech GetRandomSpeech(string tag)
    {
        var speech = speeches.FindAll(speech => string.Equals(speech.tag, tag, StringComparison.CurrentCultureIgnoreCase));
        return speech[Random.Range(0, speech.Count)];
    }

    public AudioClip GetClip(string tag)
    {
        return speeches.Find(speech => speech.tag == tag).clip;
    }

    public List<Speech.Subtitle> GetSubtitles(string tag)
    {
        return speeches.Find(speech => speech.tag == tag).subtitles;
    }

    public string GetSubtitle(string tag, float minTime)
    {
        return speeches.Find(speech => speech.tag == tag).subtitles
            .Find(subtitle => subtitle.time >= minTime).text;
    }

    [Serializable]
    public class Speech
    {
        public string tag;
        public AudioClip clip;
        public List<Subtitle> subtitles;

        public Speech(string tag, AudioClip clip, List<Subtitle> subtitles)
        {
            this.tag = tag;
            this.clip = clip;
            this.subtitles = subtitles;
        }

        [Serializable]
        public struct Subtitle
        {
            [Multiline]
            public string text;
            public float time;

            public Subtitle(string text, float time)
            {
                this.text = text;
                this.time = time;
            }
        }
    }
}