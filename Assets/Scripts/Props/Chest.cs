using UnityEngine;
using UnityEngine.Events;

namespace Refactor.Props
{
    public class Chest : Interactible
    {
        [Space]
        public bool state;

        public bool callOnEnable = true;
        public string openAnimationName = "Open";
        
        private Animator animator => GetComponent<Animator>();
        
        protected override void OnEnable()
        {
            base.OnEnable();

            onInteract.AddListener(Toggle);

            if (!callOnEnable) return;
            
            if (state)
                animator.Play(openAnimationName);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            onInteract.RemoveListener(Toggle);
        }

        public void Toggle()
        {
            if (state)
                animator.Play(openAnimationName);
        }
    }
}