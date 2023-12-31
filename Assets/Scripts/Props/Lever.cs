﻿using UnityEngine;
using UnityEngine.Events;

namespace Refactor.Props
{
    public class Lever : Interactible
    {
        [Space]
        public bool state;

        public bool callOnEnable = true;
        public UnityEvent onOn;
        public UnityEvent onOff;
        
        protected override void OnEnable()
        {
            base.OnEnable();

            onInteract.AddListener(Toggle);

            if (!callOnEnable) return;
            
            if (state)
                onOn.Invoke();
            else
                onOff.Invoke();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            onInteract.RemoveListener(Toggle);
        }

        public void Toggle()
        {
            if ((state = !state))
                onOn.Invoke();
            else
                onOff.Invoke();
        }
    }
}