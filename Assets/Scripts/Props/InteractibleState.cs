using UnityEngine;
using UnityEngine.Events;

namespace Refactor.Props
{
    public class InteractibleState : Interactible
    {
        private protected Animator animator => GetComponent<Animator>();
    }
}