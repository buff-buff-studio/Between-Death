using UnityEngine;
using UnityEngine.Events;

namespace Refactor.Props
{
    public class Lever : Interactible
    {
        public bool state;
        
        [Header("ARM")]
        public Transform arm;
        public Vector3 eulerAnglesOff = new Vector3(-30, 0, 0);
        public Vector3 eulerAnglesOn = new Vector3(30, 0, 0);
        public float armLerpSpeed = 5;

        public bool callOnEnable = true;
        public UnityEvent onOn;
        public UnityEvent onOff;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            if(arm != null)
                arm.transform.localRotation = Quaternion.Euler(state ? eulerAnglesOn : eulerAnglesOff);
            
            onInteract.AddListener(Toggle);

            if (!callOnEnable) return;
            
            if (state)
                onOn.Invoke();
            else
                onOff.Invoke();
        }

        protected override void OnDisable()
        {
            onInteract.RemoveListener(Toggle);
        }

        public void Toggle()
        {
            if ((state = !state))
                onOn.Invoke();
            else
                onOff.Invoke();
        }

        private void Update()
        {
            arm.transform.localRotation = Quaternion.Lerp(
                arm.transform.localRotation,
                Quaternion.Euler(state ? eulerAnglesOn : eulerAnglesOff),
                armLerpSpeed * Time.deltaTime
                );
        }
    }
}