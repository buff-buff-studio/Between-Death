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
        public Animator animator;
        public GameObject leftSword;
        public GameObject rightSword;
        public GameObject superSword;
        public Renderer playerRenderer;
  
        public override void OnEnable()
        {
            entity.onChangeElement.AddListener(OnChangeElement);
            OnChangeElement();
        }

        public override void OnDisable()
        {
            entity.onChangeElement.RemoveListener(OnChangeElement);
        }

        public void OnChangeElement()
        {
            var elm = entity.element;
            animator.SetLayerWeight(0, elm == Element.Order ? 1 : 0);
            animator.SetLayerWeight(1, elm == Element.Order ? 0 : 1);

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