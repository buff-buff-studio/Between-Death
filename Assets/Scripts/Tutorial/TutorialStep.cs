using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Refactor.Tutorial
{
    public class TutorialStep : MonoBehaviour
    {
        public UnityEvent onBegin;
        public UnityEvent onEnd;
        public bool isCurrent => _isCurrent;
        [SerializeField, HideInInspector]
        private bool _isCurrent = false;
        [SerializeField, HideInInspector]
        public NewTutorialController controller;

        public IngameGameInput input => controller.input;
        
        public virtual void OnBegin()
        {
            _isCurrent = true;
            onBegin.Invoke();
        }
        
        public virtual void OnEnd()
        {
            _isCurrent = false;
            onEnd.Invoke();
        }
    }
}