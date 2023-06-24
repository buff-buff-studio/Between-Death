using System.Collections.Generic;
using UnityEngine.UI;

namespace Refactor.Interface.Widgets.Settings
{
    public class SettingToggleItemWidget : SettingItemWidget
    {
        public Toggle dropdown;

        public override IEnumerable<string> GetBindingActions()
        {
            yield return "toggle_value";
        }

        public override bool DoAction(InterfaceAction action)
        {
            switch (action)
            {
                case InterfaceAction.Confirm:
                    dropdown.isOn = !dropdown.isOn;
                    return true;
                default:
                    return base.DoAction(action);
            }
        }
    }
}