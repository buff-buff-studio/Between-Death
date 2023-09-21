using System;
using System.Collections;
using System.Collections.Generic;
using Refactor;
using Refactor.Interface;
using Refactor.Interface.Windows;
using TMPro;
using UnityEngine;

public class InGameMenu : WindowManager
{
    public static InGameMenu instance;

    [SerializeField] private WindowManager windowParent;
    
    [Header("Skills References")]
    [SerializeField] private SkillManager skillManager;
    private CanvasGroup skill => skillManager.GetComponent<CanvasGroup>();

    [Space]
    [Header("Menu References")]
    [SerializeField] private TextMeshProUGUI menuText;
    [SerializeField] private TextMeshProUGUI prevMenuText;
    [SerializeField] private TextMeshProUGUI nextMenuText;

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

    protected override void OnEnable()
    {
        base.OnEnable();
        HeaderNames();
    }

    private void Update()
    {
        if (!_active) return;
        if (canvasGameInput.inputNext.triggered)
        {
            Next();
            HeaderNames();
        }else if (canvasGameInput.inputPrevious.triggered)
        {
            Previous();
            HeaderNames();
        }
    }

    private void HeaderNames()
    {
        var prev = currentWindow == 0 ? (int)(windows.Count - 1) : (int)(currentWindow - 1);
        var next = currentWindow == windows.Count - 1 ? 0 : (int)(currentWindow + 1);
        menuText.text = windows[(int)currentWindow].Tag;
        prevMenuText.text = windows[prev].Tag;
        nextMenuText.text = windows[next].Tag;
    }

    public void Menu(bool active)
    {
        Cursor.visible = active;
        Cursor.lockState = active ? CursorLockMode.None : CursorLockMode.Locked;
        IngameGameInput.CanInput = !active;
        
        if(active)
        {
            SetWindow(startWindow);
            windowParent.SetWindow(1);
        }
        else
        {
            AllWindow(false);
            windowParent.SetWindow(0);
        }
    }

    public void SkillMenu(bool active)
    {
        windowParent.SetWindow((uint)(active ? 1 : 0));
        if(active)
        {
            SetWindow("skill");
            skillManager.UpdateSkillUI();
        }
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
