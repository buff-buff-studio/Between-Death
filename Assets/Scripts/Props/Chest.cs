using UnityEngine;
using UnityEngine.Events;

namespace Refactor.Props
{
    public class Chest : InteractibleState
    {
        [Space]
        public bool canReOpen = true;

        [Space]
        public string openAnimationName = "Open";
        
        
        protected override void OnEnable()
        {
            base.OnEnable();

            onInteract.AddListener(Toggle);

            if (!callOnEnable) return;

            if (state)
            {
                animator.Play(openAnimationName);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            onInteract.RemoveListener(Toggle);
        }

        private void Toggle()
        {
            state = !state;
            if (state)
                animator.Play(openAnimationName);
        }
    }
}