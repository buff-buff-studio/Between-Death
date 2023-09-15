using System;
using System.Collections;
using System.Collections.Generic;
using Refactor;
using Refactor.Data;
using Refactor.Entities;
using Refactor.Entities.Modules;
using Refactor.Interface;
using Refactor.Misc;
using Refactor.Props;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InGameHUD : MonoBehaviour
{
    public static InGameHUD instance;

    [Space]
    [Header("UI References")]
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
    [Header("Menu References")]
    [SerializeField] private InGameMenu menu;
    
    [Space]
    [Header("Pop-Up References")]
    [SerializeField] private PopUpManager popUp;
    
    [Space]
    [Header("Interaction References")]
    [SerializeField] private RectTransform interactibleIcon;
    [SerializeField] private Interactible interactibleObject;

    [Space]
    [Header("Other References")]
    [SerializeField] private Sprite documentIcon;
    [SerializeField] private InventoryData inventoryData;
    [SerializeField] private Entity player;
    
    //Interaction References
    private bool _canInteract;
    private float _distance;

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
        
        
        UpdateSkillSlots();
        UpdateElement();
    }

    private void Update()
    {
        if (interactibleObject != null)
        {
            interactibleIcon.position = Camera.main.WorldToScreenPoint(interactibleObject.interactionPoint);
            if (IngameGameInput.InputInteract.trigger) OnInteract();
        }
        else if(popUp.isOpen)
        {
            if (IngameGameInput.InputInteract.trigger) popUp.OnClick();
            else if (IngameGameInput.InputDash.trigger) popUp.Hide();
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
            if(s >= 0)
                skillSlots[i].UpdateSlot(true, skills.GetIcon(s), skills.GetName(s));
            else
                skillSlots[i].UpdateSlot(false, null, "");
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
    }

    #region Interaction

    public void OnInteractibleEnter(Interactible interactible, float distance, bool canInteract)
    {
        if (interactible != interactibleObject && distance <= _distance) return;
        
        _distance = distance;
        _canInteract = canInteract;
        interactibleObject = interactible;
        
        interactibleIcon.sizeDelta = _canInteract ? new Vector2(100, 100) : new Vector2(50, 50);
        interactibleIcon.gameObject.SetActive(true);
    }
    
    public void OnInteractibleExit(Interactible interactible)
    {
        if (interactible != interactibleObject) return;
        
        interactibleObject = null;
        interactibleIcon.gameObject.SetActive(false);
    }

    private void OnInteract()
    {
        if (interactibleObject == null || _canInteract == false) return;
        Debug.Log("Interact");
        interactibleObject.Interact();
        if(interactibleObject.oneInteraction) interactibleIcon.gameObject.SetActive(false);
        interactibleObject = null;
        _distance = 0;
        IngameGameInput.CanInput = false;
    }

    public void OpenDocument(DocumentData doc)
    {
        popUp.Show(doc.documentName, "Novo Documento Adquirido!", documentIcon);
        inventoryData.AddUnlockedDocument(doc.GetHashCode());
    }

    public void OpenSkill(SkillData skill)
    {
        popUp.Show(skill.name, "Nova Skill Adquirida!", skill.icon, () => { menu.SkillMenu(true); });
        inventoryData.AddUnlockedSkill(skills.GetID(skill));
    }
    
    #endregion
}
