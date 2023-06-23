using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Canvas = Refactor.Interface.Canvas;

namespace Refactor.Interface.Widgets
{
    [RequireComponent(typeof(RectTransform))]
    public class Widget : MonoBehaviour
    {
        public static readonly List<Widget> Widgets = new();
        
        [HideInInspector, SerializeField]
        private RectTransform _rectTransform;
        public RectTransform rectTransform => _rectTransform;
        [HideInInspector, SerializeField]
        private Canvas _canvas;
        public Canvas canvas => _canvas;
        
        protected virtual void OnEnable()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();
            Widgets.Add(this);
        }

        protected virtual void OnDisable()
        {
            Widgets.Remove(this);
        }

        protected virtual void Update()
        {
           
        }
        
        public virtual bool DoAction(InterfaceAction action)
        {
            return false;
        }

        public virtual IEnumerable<string> GetBindingActions()
        {
            yield break;
        }

        #region Navigation
        /*
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
        */
        #endregion
    }
}