using System;
using System.Collections;
using System.Collections.Generic;
using Refactor;
using UnityEngine;

public class InGameMenu : MonoBehaviour
{
    public static InGameMenu instance;
    
    [Header("Skills References")]
    [SerializeField] private SkillManager skillManager;
    private CanvasGroup skill => skillManager.GetComponent<CanvasGroup>();
    
    [Space]
    [Header("Passive References")]
    [SerializeField] private PassiveManager passiveManager;
    private CanvasGroup passive => passiveManager.GetComponent<CanvasGroup>();
    
    [Space]
    [Header("Document References")]
    [SerializeField] private InspectDoc documentInspect;
    
    private CanvasGroup _canvasGroup => GetComponent<CanvasGroup>();

    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (_canvasGroup.alpha < 1) return;
        if (IngameGameInput.InputDash.trigger)
        {
            Menu(false);
            SkillMenu(false);
            PassiveMenu(false);
        }
    }

    public void Menu(bool active)
    {
        Cursor.visible = active;
        Cursor.lockState = active ? CursorLockMode.None : CursorLockMode.Locked;
        
        _canvasGroup.alpha = active ? 1 : 0;
        _canvasGroup.interactable = active;
        _canvasGroup.blocksRaycasts = active;
    }

    public void SkillMenu(bool active)
    {
        Menu(active);
        skillManager.UpdateInfo();
        skill.alpha = active ? 1 : 0;
        skill.interactable = active;
        skill.blocksRaycasts = active;
    }
    
    public void PassiveMenu(bool active)
    {
        Menu(active);
        passive.alpha = active ? 1 : 0;
        passive.interactable = active;
        passive.blocksRaycasts = active;
    }
}
