using System.Collections.Generic;
using TMPro;

namespace Refactor.Interface.Widgets.Settings
{
    public class SettingDropdownItemWidget : SettingItemWidget
    {
        public TMP_Dropdown dropdown;

        public override IEnumerable<string> GetBindingActions()
        {
            yield return "change_value";
        }

        public override bool DoAction(InterfaceAction action)
        {
            switch (action)
            {
                case InterfaceAction.MoveRight:
                    dropdown.value = (dropdown.value + 1)%dropdown.options.Count;
                    return true;
                case InterfaceAction.MoveLeft:
                    dropdown.value = dropdown.value > 0 ? dropdown.value - 1 : dropdown.options.Count - 1;
                    return true;
                default:
                    return base.DoAction(action);
            }
        }
    }
}