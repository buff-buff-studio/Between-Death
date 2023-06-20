using System;
using Refactor.Data;
using TMPro;
using UnityEngine;

namespace Refactor.Entities.Modules
{
    [Serializable]
    public class ElementHandlerEntityModule : EntityModule
    {
        [Header("REFERENCES")]
        public GameObject leftSword;
        public GameObject rightSword;
        public GameObject superSword;
        public Renderer playerRenderer;
  
        public override void OnEnable()
        {
            entity.onChangeElement.AddListener(OnChangeElement);
        }

        public override void OnDisable()
        {
            entity.onChangeElement.RemoveListener(OnChangeElement);
        }

        public void OnChangeElement()
        {
            var elm = entity.element;

            if (leftSword != null && rightSword != null && superSword != null)
            {
                leftSword.SetActive(elm == Element.Chaos);
                rightSword.SetActive(elm == Element.Chaos);
                superSword.SetActive(elm == Element.Order);
            }

            playerRenderer.material.color = elm.GetColor();
        }
    }
}