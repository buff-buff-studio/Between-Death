using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Refactor.Interface
{
    [RequireComponent(typeof(RectTransform))]
    public class Widget : MonoBehaviour
    {
        public static readonly List<Widget> Widgets = new();
        
        [Header("SETTINGS")]
        public bool isNavigableTarget = false;
        
        [HideInInspector]
        public RectTransform rectTransform;
        
        [Header("ANIMATION")]
        public bool hasAnimation;
        public Vector2 offset = new Vector2(20, 0);
        
        protected virtual void OnEnable()
        {
            rectTransform = GetComponent<RectTransform>();
            Widgets.Add(this);
        }

        protected virtual void OnDisable()
        {
            Widgets.Remove(this);
        }

        protected virtual void Update()
        {
           
        }
        
        #region Navigation
        public Widget GetNextNavigable(Vector2 direction)
        {
            Widget min = null;
            float minDistance = 1_000_000_000;
            
            var pos = rectTransform.position;
            foreach (var widget in Widgets)
            {
                if(widget == this) continue;
                if(!widget.isNavigableTarget) continue;
                
                var dif = widget.rectTransform.position - pos;
                var dot = Vector2.Dot(direction, dif.normalized);

                
                if (dot <= 0.15) continue;
                
                var distance = dif.magnitude * math.clamp((1.1f - dot) - 0.5f, 0.1f, 1f);
                
                Debug.DrawRay(pos, dif.normalized * distance, Color.green, 1f, false);
                
                if (distance < minDistance)
                {
                    minDistance = distance;
                    min = widget;
                }
            }

            return min;
        }
        #endregion
    }
}