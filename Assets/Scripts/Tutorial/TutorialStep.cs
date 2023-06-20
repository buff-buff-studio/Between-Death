using UnityEngine;
using UnityEngine.Events;

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
        public TutorialController tutorialController;
        
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