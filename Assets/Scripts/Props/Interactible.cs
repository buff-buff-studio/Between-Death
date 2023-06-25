using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Refactor.Props
{
    public class Interactible : MonoBehaviour
    {
        public static List<Interactible> Interactibles = new();
        public Transform interactionPointOffset;
        public float radius = 2f;
        public float time = 0f;
        
        public Vector3 offset;
        
        [Space]
        public UnityEvent onInteract;
        
        public Vector3 interactionPoint => interactionPointOffset != null ? interactionPointOffset.position : transform.position;        
        protected virtual void OnEnable()
        {
            Interactibles.Add(this);
        }

        protected virtual void OnDisable()
        {
            Interactibles.Remove(this);
        }

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + offset, radius);
        }
    }
}