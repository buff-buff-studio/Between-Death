using System;
using System.Collections;
using System.Collections.Generic;
using Refactor;
using Refactor.Interface;
using Refactor.Interface.Windows;
using UnityEngine;

public class InGameMenu : WindowManager
{
    public static InGameMenu instance;

    [SerializeField] private WindowManager windowParent;
    
    [Header("Skills References")]
    [SerializeField] private SkillManager skillManager;
    private CanvasGroup skill => skillManager.GetComponent<CanvasGroup>();
    
    [Space]
    [Header("Passive References")]
    [SerializeField] private PassiveManager passiveManager;
    private CanvasGroup passive => passiveManager.GetComponent<CanvasGroup>();
    
    [Space]
    [Header("Document References")]
    [SerializeField] private DocumentManager documentManager;

    [SerializeField] private CanvasGameInput canvasGameInput;
    private CanvasGroup _canvasGroup => GetComponent<CanvasGroup>();

    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (!_active) return;
        if (canvasGameInput.inputCancel.triggered)
        {
            IngameGameInput.CanInput = true;
            windowParent.SetWindow(0);
            SkillMenu(false);
            PassiveMenu(false);
            DocumentMenu(false);
        }else if (canvasGameInput.inputNext.triggered)
        {
            Next();
        }else if (canvasGameInput.inputPrevious.triggered)
        {
            Previous();
        }
    }

    public void Menu(bool active)
    {
        Cursor.visible = active;
        Cursor.lockState = active ? CursorLockMode.None : CursorLockMode.Locked;
        IngameGameInput.CanInput = !active;
        
        if(active) windowParent.SetWindow(1);
        else AllWindow(false);
    }

    public void SkillMenu(bool active)
    {
        windowParent.SetWindow((uint)(active ? 1 : 0));
        if(active) SetWindow("skill");
        else AllWindow(false);
    }
    
    public void PassiveMenu(bool active)
    {
        windowParent.SetWindow((uint)(active ? 1 : 0));
        if(active) SetWindow("passive");
        else AllWindow(false);
    }

    public void DocumentMenu(bool active)
    {
        windowParent.SetWindow((uint)(active ? 1 : 0));
        if(active) SetWindow("document");
        else AllWindow(false);
    }
}
