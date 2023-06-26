using Refactor.Audio;
using Refactor.Data.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace Refactor.Interface.Data
{
    public class SliderDataController : MonoBehaviour
    {
        public Slider slider;
        public Value<float> value;

        public void OnEnable()
        {
            slider.onValueChanged.AddListener(_HandleValue);
            if (value.hasVariable)
            {
                value.currentVariable.onChanged.AddListener(_HandleValueInverse);
                slider.value = value.value;
            }
        }

        private void OnDisable()
        {
            slider.onValueChanged.RemoveListener(_HandleValue);
            if (value.hasVariable)
                value.currentVariable.onChanged.RemoveListener(_HandleValueInverse);
        }

        private void _HandleValue(float v)
        {
            value.value = v;
        }

        private void _HandleValueInverse()
        {
            slider.value = value.value;
            AudioSystem.PlaySound("ui_click");
        }
    }
}