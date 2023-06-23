using System.Collections.Generic;
using UnityEngine.UI;

namespace Refactor.Interface.Widgets.Settings
{
    public class SettingSliderItemWidget : SettingItemWidget
    {
        public Slider slider;

        public override IEnumerable<string> GetBindingActions()
        {
            yield return "change_value";
        }

        public override bool DoAction(InterfaceAction action)
        {
            switch (action)
            {
                case InterfaceAction.MoveRight:
                    slider.value++;
                    return true;
                case InterfaceAction.MoveLeft:
                    slider.value--;
                    return true;
                default:
                    return base.DoAction(action);
            }
        }
    }
}