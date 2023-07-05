using UnityEngine;
using UnityEngine.Events;

namespace Refactor.Props
{
    public class InteractibleState : Interactible
    {
        [Space]
        public bool state;
        public bool callOnEnable = true;

        private protected Animator animator => GetComponent<Animator>();
    }
}