using UnityEngine;

namespace Refactor.Data.Variables
{
    [CreateAssetMenu(fileName = "Inventory", menuName = "Refactor/Data/Inventory", order = 1)]
    public class InventoryVar : Variable<InventoryData>
    {
        public override void Reset()
        {
            if (shouldReset)
            {
                var data = ScriptableObject.CreateInstance<InventoryData>();
                data.SetData(defaultValue);
                Value = data;
            }
        }
    }
}