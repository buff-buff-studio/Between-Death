using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Singleton

    private static GameManager _instance;
    public static GameManager Instance => _instance ? _instance : FindObjectOfType<GameManager>();

    #endregion

    [Header("Essentials")]
    //public HumanEntity player;
    public Camera camRef;
    public Canvas canvas;
    public RectTransform interactIcon;
    public PlayerInput playerInput;
    //private GameInputs _inputs;
    
    [Space]
    [Header("Health Bar")]
    public Image healthBar;
    //public Variable<float> healthValue;
    
    [Space]
    [Header("Element")]
    public Image elementIcon;
    public Sprite[] elementSprites;
    //public Element[] elements;
    
    [Space]
    [Header("Skills")]
    public Image[] skillIcons;
    
    [Space]
    [Header("Items")]
    public ItemData itemData;
    public Image itemIcon;
    //public DocumentVariable _documentVariable;

    private InteractiveObject interactiveObj;
    private GameObject SkillBtt(int i) => skillIcons[i].transform.GetChild(1).gameObject;

    public static RectTransform GetInteractIcon => Instance.interactIcon;
    public static Image GetInteractIconImage => Instance.interactIcon.GetComponent<Image>();

    private void Awake()
    {
        camRef = Camera.main;
        //_inputs = new GameInputs();
        playerInput ??= FindObjectOfType<PlayerInput>();

        //TODO:: REMOVE THIS
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
 
    public void OnEnable()
    {
        //_inputs.Enable();
        
        //player.onElementChange += ChangeElementDisplay;
        //player.onHeal += HealthBarDisplay;
        //player.onDamage += HealthBarDisplay;
        
        ChangeElementDisplay();
        HealthBarDisplay();
    }

    public void OnDisable()
    {
        //player.onElementChange -= ChangeElementDisplay;
        //player.onHeal -= HealthBarDisplay;
    }

    public void HUDSetVisible(bool visible)
    {
        canvas.enabled = visible;
    }
    
    public void AddItemToInventory(ItemData data)
    {
        itemData = data;
        itemIcon.enabled = true;
        itemIcon.sprite = data.itemIcon;
    }
    
    public void RemoveItemFromInventory()
    {
        itemData = null;
        itemIcon.enabled = false;
    }
    
    public void InteractIconPos(Vector3 pos, InteractiveObject intObj = default)
    {
        if (interactiveObj.distance < intObj.distance) return;
        
        interactiveObj = intObj;
        interactIcon.position = Camera.main.WorldToScreenPoint(pos);
    }
    
    public void InteractIconSize(float size, InteractiveObject intObj = default)
    {
        if (interactiveObj.distance < intObj.distance) return;
        interactIcon.sizeDelta = new Vector2(size, size);
    }
    
    public void InteractIconVisibility(bool visible, InteractiveObject intObj = default)
    {
        interactiveObj ??= intObj;
        if (interactiveObj != null && interactiveObj.distance < intObj.distance) return;
        
        interactiveObj = intObj;
        if(!visible) interactiveObj = null;
        interactIcon.GetComponent<Image>().enabled = visible;
    }

    private void HealthBarDisplay(float f = 0)
    {
        //healthBar.fillAmount = healthValue.Value / 100f;
    }

    private void ChangeElementDisplay()
    {
        /*
        int index = Array.IndexOf(elements, player.element);
        
        if(index == -1)
        {
            elementIcon.sprite = null;
            return;
        }

        elementIcon.sprite = elementSprites[index];
        */
    }
    
    private void IsShowingSkill()
    {
        if (true)
        {
            SkillBtt(0).SetActive(true);
            SkillBtt(1).SetActive(true);
            SkillBtt(2).SetActive(true);
            SkillBtt(3).SetActive(false);
        }
        else
        {
            SkillBtt(0).SetActive(false);
            SkillBtt(1).SetActive(false);
            SkillBtt(2).SetActive(false);
            SkillBtt(3).SetActive(true);
        }
    }
}
