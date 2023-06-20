using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Refactor.Props
{
    public class Interactible : MonoBehaviour
    {
        public static List<Interactible> Interactibles = new();
        public float radius = 2f;
        public float time = 0f;
        
        public UnityEvent onInteract;

        protected virtual void OnEnable()
        {
            Interactibles.Add(this);
        }

        protected virtual void OnDisable()
        {
            Interactibles.Remove(this);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}