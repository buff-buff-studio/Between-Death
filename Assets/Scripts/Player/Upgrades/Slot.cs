using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Values")]
    [SerializeField] private int id;
    
    [Header("Visuals")]
    [SerializeField] private Image icon;
    [SerializeField] private TMPro.TextMeshProUGUI name;

    protected bool active = true;
    //TODO:: Remove after implementing the widget system.
    public Image hover;
    
    public Sprite sprite
    {
        get => icon.sprite;
        set
        {
            if(value == null)
                icon.enabled = false;
            else
            {
                icon.sprite = value;
                icon.enabled = true;
            }
        }
    }
    
    public string text
    {
        get => name.text;
        set
        {
            if (name == null) return;
            name.text = value;
        }
    }
    
    public int ID
    {
        get => id;
        set => id = value;
    }

    protected virtual void Start()
    {
        if (hover != null) hover.enabled = false;
        UpdateSlot(id);
    }

    public virtual void UpdateSlot() => UpdateSlot(ID);
    public virtual void UpdateSlot(int id) => this.id = id;
    
    public virtual void UpdateSlot(bool active, Sprite sprite, string text)
    {
        gameObject.SetActive(active);
        this.active = active;
        this.sprite = sprite;
        this.text = text;
    }

    public virtual void OnPointerEnter(PointerEventData eventData) { if (hover != null && active) hover.enabled = true; }
    
    public virtual void OnPointerExit(PointerEventData eventData) { if (hover != null) hover.enabled = false; }
}
