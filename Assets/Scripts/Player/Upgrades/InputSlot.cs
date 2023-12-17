using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputSlot : Slot
{
    [Space][Header("Input")]
    [SerializeField] private uint slot;
    [SerializeField] private Button button;

    private enum InputType
    {
        Skill,
        Passive
    }
    [SerializeField] private InputType type;
    
    protected override void Start()
    {
        base.Start();
        button.onClick.AddListener(OnClick);
    }

    public override void UpdateSlot(int id)
    {
        base.UpdateSlot(id);
        button.interactable = true;
        text = type switch
        {
            InputType.Skill => SkillManager.instance.skills.GetName(id),
            InputType.Passive => PassiveManager.instance.passives.GetName(id),
            _ => null
        };

        if (id < 0)
        {
            sprite = null;
            return;
        }
        sprite = type switch
        {
            InputType.Skill => SkillManager.instance.skills.GetIcon(id),
            InputType.Passive => PassiveManager.instance.passives.GetIcon(id),
            _ => null
        };
    }
    
    public void OnClick()
    {
        switch (type)
        {
            case InputType.Skill:
                SkillManager.instance.Equip(ID, slot);
                break;
            case InputType.Passive:
                PassiveManager.instance.Equip(ID, slot);
                break;
            default:
                break;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        button ??= GetComponent<Button>();
    }    
#endif
    public override void OnPointerEnter(PointerEventData eventData)
    {
        hover.enabled = true;
        
        if (ID < 0) return;
        switch (type)
        {
            case InputType.Skill:
                SkillManager.instance.UpdateInfo(ID);
                break;
            case InputType.Passive:
                PassiveManager.instance.UpdateInfo(ID);
                break;
            default:
                break;
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        hover.enabled = false;
    }
}
