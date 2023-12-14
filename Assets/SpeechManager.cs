using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class SpeechManager : MonoBehaviour
{
    private static SpeechManager instance;

    [SerializeField]
    private PlayerSpeechData speechData;
    [SerializeField]
    private TextMeshProUGUI subtitleText;
    [SerializeField]
    private GameObject subtitleArea;
    [SerializeField]
    private AudioSource audioSource;


    [SerializeField] private PlayerSpeechData.Speech _speech;
    private PlayerSpeechData.Speech speech
    {
        get => _speech;
        set
        {
            _speech = value;
            _speechTag = value == null ? "" : value.tag;
        }
    }

    public static PlayerSpeechData data => instance.speechData;
    private string _speechTag = "";

    private void Awake()
    {
        CreateInstance();
    }

    private void Start()
    {
        audioSource ??= transform.AddComponent<AudioSource>();
        subtitleText.text = "";
    }

    private void CreateInstance()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    public void PlaySpeech(string speechTag)
    {
        if (this._speechTag.Equals(speechTag)) return;

        speech = speechData.GetSpeech(speechTag);
        StartCoroutine(PlaySpeech());
    }

    private void PlaySpeech(PlayerSpeechData.Speech data)
    {
        if (_speechTag.Equals(data.tag)) return;

        speech = data;
        StartCoroutine(PlaySpeech());
    }

    public static void Play(string speechTag)
    {
        instance.PlaySpeech(speechTag);
    }

    public static void PlayRandom(string speechTag)
    {
        instance.PlaySpeech(data.GetRandomSpeech(speechTag));
    }

    public static bool Exists(string speechTag)
    {
        while (true)
        {
            if (instance != null) return instance.speechData.GetSpeech(speechTag) != null;
            FindObjectOfType<SpeechManager>().CreateInstance();
            continue;

            break;
        }
    }

    private IEnumerator PlaySpeech()
    {
        audioSource.clip = speech.clip;
        audioSource.Play();
        var time = 0f;

        foreach (var subtitle in speech.subtitles)
        {
            yield return new WaitForSeconds(subtitle.time - time);

            time = subtitle.time;

            subtitleArea.SetActive(subtitle.text.Trim() != "");
            subtitleText.text = subtitle.text;
        }

        while (audioSource.isPlaying) yield return null;

        subtitleArea.SetActive(false);
        subtitleText.text = "";
        speech = null;
    }
}
