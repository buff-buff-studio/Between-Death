using UnityEngine;
using UnityEngine.Events;

namespace Refactor.Props
{
    public class InteractableState : Interactable
    {
        private protected Animator animator => GetComponent<Animator>();
    }
}