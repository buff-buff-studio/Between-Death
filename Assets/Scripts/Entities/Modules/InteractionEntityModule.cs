using System;
using Refactor.Misc;
using Refactor.Props;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Refactor.Entities.Modules
{
    [Serializable]
    public class InteractionEntityModule : EntityModule
    {
        public Vector3 interactionOffset;
        [FormerlySerializedAs("currentInteractible")] public Interactable currentInteractable;
        public GameObject prefabInteractionDisplay;
        public InteractionDisplay interactionDisplay;
        private Interactable _lastFrameInteractable;
        
        [SerializeField, HideInInspector]
        public PlayerControllerEntityModule controllerEntityModule;
        public float time = 0;
        public bool needToRestart = true;

        public override void OnEnable()
        {
            controllerEntityModule = entity.GetModule<PlayerControllerEntityModule>();
            if (interactionDisplay == null)
            {
                var go = Object.Instantiate(prefabInteractionDisplay, entity.transform);
                interactionDisplay = go.GetComponent<InteractionDisplay>();
            }
        }

        public override void OnDisable()
        {
            if (interactionDisplay != null)
            {
                Object.Destroy(interactionDisplay.gameObject);
                interactionDisplay = null;
            }
        }

        public override void UpdateFrame(float deltaTime)
        {
            var canInteract = controllerEntityModule.state == PlayerState.Default && entity.isGrounded;
            
            if (currentInteractable != null && canInteract)
            {
                interactionDisplay.gameObject.SetActive(true);
                interactionDisplay.transform.position = currentInteractable.interactionPoint;
            }
            else
            {
                interactionDisplay.gameObject.SetActive(false);
            }

            Interactable frameInteractable = null;

            if (IngameGameInput.InputInteract.value)
            {
                if (canInteract && currentInteractable != null && currentInteractable.enabled)
                {
                    frameInteractable = currentInteractable;
                }
            }
            else
                needToRestart = false;

            if (frameInteractable == _lastFrameInteractable)
            {
                if (frameInteractable != null && !needToRestart)
                {
                    /*
                    if ((time += deltaTime) > frameInteractible.time)
                    {
                        frameInteractible.onInteract.Invoke();
                        needToRestart = true;
                        time = 0;
                    }

                    interactionDisplay.progress = time / frameInteractible.time;
                    */
                }
                else
                    interactionDisplay.progress = time = 0;
            }
            else
                interactionDisplay.progress = time = 0;
            
            _lastFrameInteractable = frameInteractable;
        }
    }
}