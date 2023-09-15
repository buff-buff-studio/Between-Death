using Refactor.Props;
using UnityEngine;

public class GenericItemData : ChestItem
{
    [Header("About")]
    public string name;
    [TextArea(3, 10)]
    public string description;
    
    [Space]
    [Header("Visuals")]
    public Sprite icon;
}
