using System;
using System.Collections;
using System.Collections.Generic;
using Refactor.Data.Variables;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpeechTrigger : MonoBehaviour
{
    [SerializeField]
    private string speechTag;
    [SerializeField]
    private BoolVariable speechTriggered;

    private void Awake()
    {
        if(speechTriggered == null)
        {
            speechTriggered ??= ScriptableObject.CreateInstance<BoolVariable>();
            speechTriggered.Value = false;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if(speechTriggered.Value)
        {
            gameObject.SetActive(false);
            return;
        }

        if (col.CompareTag("Player"))
        {
            SpeechManager.Play(speechTag);
            speechTriggered.Value = true;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        var collider = GetComponent<Collider>();
        if(!collider.isTrigger) collider.isTrigger = true;
        if(!SpeechManager.Exists(speechTag)) Debug.LogWarning($"Speech tag {speechTag} does not exist!", SpeechManager.data);
    }
#endif
}
