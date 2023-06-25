using System;
using Refactor.Misc;
using Refactor.Props;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Refactor.Entities.Modules
{
    [Serializable]
    public class InteractionEntityModule : EntityModule
    {
        public Vector3 interactionOffset;
        public Interactible currentInteractible;
        public GameObject prefabInteractionDisplay;
        public InteractionDisplay interactionDisplay;
        private Interactible _lastFrameInteractible;
        
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
            
            if (currentInteractible != null && canInteract)
            {
                interactionDisplay.gameObject.SetActive(true);
                interactionDisplay.transform.position = currentInteractible.interactionPoint;
            }
            else
            {
                interactionDisplay.gameObject.SetActive(false);
            }

            Interactible frameInteractible = null;

            if (IngameGameInput.InputInteract.value)
            {
                if (canInteract && currentInteractible != null && currentInteractible.enabled)
                {
                    frameInteractible = currentInteractible;
                }
            }
            else
                needToRestart = false;

            if (frameInteractible == _lastFrameInteractible)
            {
                if (frameInteractible != null && !needToRestart)
                {
                    if ((time += deltaTime) > frameInteractible.time)
                    {
                        frameInteractible.onInteract.Invoke();
                        needToRestart = true;
                        time = 0;
                    }

                    interactionDisplay.progress = time / frameInteractible.time;
                }
                else
                    interactionDisplay.progress = time = 0;
            }
            else
                interactionDisplay.progress = time = 0;
            
            _lastFrameInteractible = frameInteractible;
        }
    }
}