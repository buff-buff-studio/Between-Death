using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Refactor.Interface
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ClosableCanvasGroup : MonoBehaviour
    {
        public const float ALPHA_THRESHOLD = 0.002f;
        
        [SerializeField]
        private bool open;
        public float lerpSpeed = 5f;
        
        private CanvasGroup _canvasGroup;

        private void OnValidate()
        {
            if(_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
            SetOpen(open);
        }

        public void OnEnable()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            gameObject.SetActive(open);
            _canvasGroup.alpha = open ? 1 : 0;
        }

        public void SetOpen(bool isOpen)
        {
            open = isOpen;
            gameObject.SetActive(open || _canvasGroup.alpha > ALPHA_THRESHOLD);
        }

        private void Update()
        {
            _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, open ? 1 : 0, lerpSpeed * Time.deltaTime);

            if (open || _canvasGroup.alpha > ALPHA_THRESHOLD) return;
            gameObject.SetActive(false);
        }
    }
}