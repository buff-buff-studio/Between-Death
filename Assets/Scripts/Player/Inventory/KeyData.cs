using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Key", menuName = "RPG/Key")]
public class KeyData : GenericItemData
{
    public bool oneTimeUse = false;
    
    public void Use()
    {
        if (oneTimeUse) Destroy(this);
    }
}
