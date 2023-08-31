using UnityEngine;

public class GenericItemData : ScriptableObject
{
    [Header("About")]
    public string name;
    [TextArea(3, 10)]
    public string description;
    
    [Space]
    [Header("Visuals")]
    public Sprite icon;
}
