using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Refactor;
using Refactor.Data;
using Refactor.Entities;
using Refactor.Entities.Modules;
using Refactor.Interface;
using Refactor.Interface.Windows;
using Refactor.Misc;
using Refactor.Props;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InGameHUD : WindowManager
{
    public static InGameHUD instance;

    [Space]
    [Header("UI")]
    [SerializeField] private Image lifeBar;
    [SerializeField] private AnimationCurve lifeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Space]
    [SerializeField] private Slot[] skillSlots;
    [SerializeField] private SkillManager skillManager;
    private SkillList skills => skillManager.skills;
    private List<int> equippedSkills => inventoryData.GetEquippedSkills;
    
    [Space]
    [SerializeField] private Image elementIcon;
    [SerializeField] private Sprite orderIcon;
    [SerializeField] private Sprite chaosIcon;
    
    [Space]
    [Header("POP-UP")]
    [SerializeField] private PopUpManager popUp;
    
    [Space]
    [Header("INTERACTION")]
    [SerializeField] private RectTransform interactibleIcon;
    [FormerlySerializedAs("interactibleObject")] [SerializeField] private Interactable interactableObject;

    [Space]
    [Header("MENU")]
    [SerializeField] private Window quitDialogWindow;
    [SerializeField] private Window settingsDialogWindow;
    [SerializeField] private Window deathDialogWindow;
    [SerializeField] private InGameMenu menu;
    [SerializeField] private GameObject skillDash;
    
    [Space]
    [Header("OTHERS")]
    [SerializeField] private Sprite documentIcon;
    [SerializeField] private InventoryData inventoryData;
    [SerializeField] private Entity player;
    [SerializeField] private CanvasGameInput canvasGameInput;
    [SerializeField] private Camera _camera;
    
    private DocumentList documents => inventoryData.GetDocumentList;
    public bool HasKey(KeyData key) => inventoryData.HasKey(key);
    public void UseKey(KeyData key) => inventoryData.UseKey(key);
    
    //Interaction References
    private bool _canInteract;
    private float _distance;
    private bool _isDead = false;

    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(this);
        
        interactibleIcon.gameObject.SetActive(false);
    }

    private void Start()
    {
        player.onChangeElement.AddListener(UpdateElement);
        var module = player.GetModule<HealthEntityModule>();
        module.onHealthChange.AddListener((h) =>
        {
            var hlt = (IHealth)module;
            UpdateLife(h, hlt.maxHealth);
        });
        
        //UpdateSkillSlots();
        UpdateElement();
    }

    private void OnEnable()
    {
        IngameGameInput.CanInput = true;
    }
    
    public void QuitGame()
    {
        Time.timeScale = 1;
        quitDialogWindow.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        StartCoroutine(_QuitGame());
    }

    private IEnumerator _QuitGame()
    {
        yield return new WaitForSeconds(1f);
        LoadingScreen.LoadScene("Scenes/Menu");
    }

    public void CloseQuitGame()
    {
        quitDialogWindow.Close();
        StartCoroutine(_CloseQuitGame());
    }
        
    private IEnumerator _CloseQuitGame()
    {
        yield return new WaitForSeconds(0.5f);
        (canvasGameInput as IngameGameInput)!.canInput = true;
        Debug.Log("back");
    }

    public void OpenDeathDialog()
    {
        deathDialogWindow.Open();
        _isDead = true;
        (canvasGameInput as IngameGameInput)!.canInput = false;
    }

    public void CloseDeathDialog()
    {
        deathDialogWindow.Close();
        _isDead = false;
        (canvasGameInput as IngameGameInput)!.canInput = true;
    }

    public void OpenPauseMenu()
    {
        quitDialogWindow.Open();
        (canvasGameInput as IngameGameInput)!.canInput = false;
    }

    public void ClosePauseMenu()
    {
        quitDialogWindow.Close();
        (canvasGameInput as IngameGameInput)!.canInput = true;
    }

    private void Update()
    {
        if(_isDead) return;

        UpdateSkillSlots();
        
        if(canvasGameInput.inputStart.triggered)
        {
            if((canvasGameInput as IngameGameInput)!.canInput
             && !quitDialogWindow.isOpen)
            {
                OpenPauseMenu();
                return;
            }else if(settingsDialogWindow.isOpen)
            {
                settingsDialogWindow.Close();
                return;
            }
        }
        
        if(canvasGameInput.inputInventory.triggered && canvasGameInput.canMenu)
        {
            menu.Menu(_active);
        }

        if (!_active) return;
        if (interactableObject != null)
        {
            interactibleIcon.position = _camera.WorldToScreenPoint(interactableObject.interactionPoint);
            if (IngameGameInput.InputInteract.trigger) OnInteract();
        }
        else if(popUp.isOpen)
        {
            if (canvasGameInput.inputCancel.triggered) popUp.OnClick();
            else if (canvasGameInput.inputConfirm.triggered) popUp.Hide();
        }
    }
    
    public void UpdateLife(float life, float maxLife)
    {
        StartCoroutine(UpdateLifeBar(lifeCurve.Evaluate(life / maxLife)));
    }
    
    private IEnumerator UpdateLifeBar(float value)
    {
        float time = 0;
        float start = lifeBar.fillAmount;
        float end = value;
        
        while (time < 1)
        {
            time += Time.deltaTime;
            lifeBar.fillAmount = Mathf.Lerp(start, end, lifeCurve.Evaluate(time));
            yield return null;
        }
    }
    
    public void UpdateSkillSlots()
    {
        for (int i = 0; i < skillSlots.Length; i++)
        {
           
            var s = equippedSkills[i];
            if (s >= 0)
            {
                var skill = skills.skills[s];
                skillSlots[i].UpdateSlot(true, skill.icon, skill.name, skill.actualCooldown / skill.cooldown);
            }
            else
                skillSlots[i].UpdateSlot(false, null, "", 0);
        }
    }

    public void UpdateElement()
    {
        elementIcon.sprite = player.element switch
        {
            Element.Chaos => chaosIcon,
            Element.Order => orderIcon,
            _ => elementIcon.sprite
        };
        
        skillDash.SetActive(player.element == Element.Chaos);
    }

    #region Interaction

    public void OnInteractibleEnter(Interactable interactable, float distance, bool canInteract)
    {
        if(interactableObject != null) _distance = Vector3.Distance(player.transform.position, interactableObject.interactionPoint);
        if (interactable != interactableObject && distance <= _distance) return;
        
        _distance = distance;
        _canInteract = canInteract;
        interactableObject = interactable;
        
        interactibleIcon.sizeDelta = _canInteract ? new Vector2(100, 100) : new Vector2(50, 50);
        interactibleIcon.gameObject.SetActive(true);
    }
    
    public void OnInteractibleExit(Interactable interactable)
    {
        if (interactable != interactableObject) return;
        
        interactableObject = null;
        interactibleIcon.gameObject.SetActive(false);
        _distance = 0;
    }

    private void OnInteract()
    {
        if (interactableObject == null || _canInteract == false) return;



        Debug.Log("Interact");
        interactableObject.Interact();
        if(interactableObject.oneInteraction)
        {
            interactibleIcon.gameObject.SetActive(false);
            interactableObject = null;
            _distance = 0;
        }
    }

    public void OpenDocument(DocumentData doc)
    {
        popUp.Show(doc.documentName, "Novo Documento Adquirido!", documentIcon, () => { menu.DocumentMenu(true); });

        IngameGameInput.CanInput = false;
        inventoryData.AddUnlockedDocument(documents.GetID(doc));
    }

    public void OpenSkill(SkillData skill)
    {
        popUp.Show(skill.name, "Nova Skill Adquirida!", skill.icon, () => { menu.SkillMenu(true); });

        IngameGameInput.CanInput = false;
        inventoryData.AddUnlockedSkill(skills.GetID(skill));
    }

    public void OpenItem(KeyData item)
    {
        popUp.Show(item.name, item.description, item.icon, false, true);

        IngameGameInput.CanInput = false;
        inventoryData.AddKey(item);
    }
    
    #endregion
}
