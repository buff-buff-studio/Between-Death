using System;
using Refactor.Data.Variables;
using Refactor.Entities;
using Refactor.Entities.Modules;
using Refactor.Props;
using UnityEngine;

namespace Refactor
{
    public class GameController : MonoBehaviour
    {
        [Header("REFERENCES")]
        public Entity player;
        public Variable[] toReset;
        
        private InteractionEntityModule _interactionPlayerModule;
        private void Awake()
        {
            foreach(var v in toReset)
                v.Reset();
        }

        
        public void OnEnable()
        {
            if (player != null)
            {
                _interactionPlayerModule = player.GetModule<InteractionEntityModule>();
                if(_interactionPlayerModule != null)
                    InvokeRepeating(nameof(UpdateInteractibles), 0f, 0.1f);
            }
        }

        public void UpdateInteractibles()
        {
            Interactible min = null;
            var minDistance = .0f;

            var pos = player.transform.position + _interactionPlayerModule.interactionOffset;

            foreach (var i in Interactible.Interactibles)
            {
                var distance = Vector3.Distance(pos, i.transform.position);

                if (distance < i.radius && (min == null || distance < minDistance))
                {
                    minDistance = distance;
                    min = i;
                }
            }

            _interactionPlayerModule.currentInteractible = min;
        }
    }
}