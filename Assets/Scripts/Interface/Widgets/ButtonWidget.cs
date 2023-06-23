using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Refactor.Interface.Widgets
{
    public class ButtonWidget : Widget, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public TMP_Text label;
        public Color colorNormal = Color.white;
        public Color colorHover = Color.gray;
        public Color colorClick = Color.red;
        public int state = 0;
        public UnityEvent onPress;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            label.color = colorNormal;
            state = 0;
        }

        protected override void Update()
        {
            base.Update();
            label.color = Color.Lerp(label.color, state switch
            {
                2 => colorClick,
                1 => colorHover,
                _ => colorNormal
            }, Time.deltaTime * 12f);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onPress.Invoke();
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            state = math.max(state, 1);
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            state = 0;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            state = 2;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            state = math.min(state, 1);
        }

        public override bool DoAction(InterfaceAction action)
        {
            if (action == InterfaceAction.Confirm)
            {
                StartCoroutine(_DoClick());
                return true;
            }

            return false;
        }

        private IEnumerator _DoClick()
        {
            onPress.Invoke();
            state = 2;
            yield return new WaitForSeconds(0.1f);
            state = 0;
        }
        
        public override IEnumerable<string> GetBindingActions()
        {
            yield return "confirm";
        }
    }
}