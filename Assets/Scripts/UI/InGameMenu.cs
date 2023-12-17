using System;
using System.Collections;
using System.Collections.Generic;
using Refactor;
using Refactor.Interface;
using Refactor.Interface.Windows;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class InGameMenu : WindowManager
{
    public static InGameMenu instance;

    [SerializeField] private WindowManager windowParent;
    
    [Header("Skills References")]
    [SerializeField] private SkillManager skillManager;
    private CanvasGroup skill => skillManager.GetComponent<CanvasGroup>();

    [Space]
    [Header("Menu References")]
    [SerializeField] private Label menuText;
    [SerializeField] private Label prevMenuText;
    [SerializeField] private Label nextMenuText;

    [Space]
    [Header("Passive References")]
    [SerializeField] private PassiveManager passiveManager;
    private CanvasGroup passive => passiveManager.GetComponent<CanvasGroup>();
    
    [Space]
    [Header("Document References")]
    [SerializeField] private DocumentManager documentManager;

    [SerializeField] private CanvasGameInput canvasGameInput;
    private CanvasGroup _canvasGroup => GetComponent<CanvasGroup>();
    public UnityEvent<bool> onChangeMenuOpen;

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
        menuText.SetCache($"[ui.{windows[(int)currentWindow].Tag}.title]");
        prevMenuText.SetCache($"[ui.{windows[prev].Tag}.title]");
        nextMenuText.SetCache($"[ui.{windows[next].Tag}.title]");
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
            onChangeMenuOpen.Invoke(true);
        }
        else
        {
            AllWindow(false);
            windowParent.SetWindow(0);
            onChangeMenuOpen.Invoke(false);
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
        if(active)
        {
            SetWindow("document");
            documentManager.UpdateInventory();
        }
        else AllWindow(false);
    }
}
