using System;
using Refactor.Audio;
using Refactor.Data.Variables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Refactor.Interface.Data
{
    public class DropdownDataController : MonoBehaviour
    {
        public TMP_Dropdown toggle;
        public Value<int> value;

        public void OnEnable()
        {
            toggle.onValueChanged.AddListener(_HandleValue);
            if (value.hasVariable)
            {
                value.currentVariable.onChanged.AddListener(_HandleValueInverse);
                toggle.value = value.value;
            }
        }

        private void OnDisable()
        {
            toggle.onValueChanged.RemoveListener(_HandleValue);
            if (value.hasVariable)
                value.currentVariable.onChanged.RemoveListener(_HandleValueInverse);
        }

        private void _HandleValue(int v)
        {
            value.value = v;
        }

        private void _HandleValueInverse()
        {
            toggle.value = value.value;
            AudioSystem.PlaySound("ui_click");
        }
    }
}