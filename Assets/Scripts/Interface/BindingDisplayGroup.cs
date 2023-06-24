using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Refactor.Interface
{
    public class BindingDisplayGroup : MonoBehaviour
    {
        [Serializable]
        public class BindingDisplayItem
        {
            public string name;
            public BindingDisplay display;
        }

        public BindingDisplayItem[] items;

        public void FixedUpdate()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetRectTransform());
        }

        public void SetVisible(List<string> names)
        {
            foreach (var bdi in items)
            {
                bdi.display.gameObject.SetActive(names.Contains(bdi.name));
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }
    }
}