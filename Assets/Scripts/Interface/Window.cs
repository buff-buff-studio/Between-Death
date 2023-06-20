using UnityEngine;
using UnityEngine.Events;

namespace Refactor.Interface
{
    public class Window : Widget
    {
        public string id;

        public UnityEvent onBack;
    
        public Widget GetFirstWidget()
        {
            return null;
        }
    }
}