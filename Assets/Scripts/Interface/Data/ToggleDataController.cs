using Refactor.Data.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace Refactor.Interface.Data
{
    public class ToggleDataController : MonoBehaviour
    {
        public Toggle toggle;
        public Value<bool> value;

        public void OnEnable()
        {
            toggle.onValueChanged.AddListener(_HandleValue);
            if (value.hasVariable)
            {
                value.currentVariable.onChanged.AddListener(_HandleValueInverse);
                _HandleValueInverse();
            }
        }

        private void OnDisable()
        {
            toggle.onValueChanged.RemoveListener(_HandleValue);
            if (value.hasVariable)
                value.currentVariable.onChanged.RemoveListener(_HandleValueInverse);
        }

        private void _HandleValue(bool v)
        {
            value.value = v;
        }

        private void _HandleValueInverse()
        {
            toggle.isOn = value.value;
        }
    }
}