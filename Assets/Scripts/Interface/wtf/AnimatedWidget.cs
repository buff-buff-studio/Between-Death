using System;
using System.Collections;
using UnityEngine;

namespace Refactor.Interface
{
    [RequireComponent(typeof(RectTransform))]
    public class AnimatedWidget : MonoBehaviour
    {
        public bool state = false;
        public float speed = 4f;
        public Vector2 offset = new Vector2(50, 0);
        
        [SerializeField, HideInInspector]
        private CanvasGroup canvasGroup;
        [HideInInspector]
        public RectTransform rectTransform;
        
        private void OnEnable()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();

            canvasGroup.alpha = state ? 1 : 0;

            StartCoroutine(HandleIt());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        public IEnumerator HandleIt()
        {
            yield return new WaitForSeconds(0.05F);
            while (true)
            {
                var cur = state;
                var pos = rectTransform.anchoredPosition;

                while (state == cur)
                {
                    var deltaTime = Time.deltaTime;
                    var alpha = canvasGroup.alpha;
                    canvasGroup.alpha = Mathf.Lerp(alpha, state ? 1 : 0, deltaTime * speed);
                    canvasGroup.interactable = state && alpha > 0.9f;
                    
                    var toApply = (1f - alpha) * offset;
                    rectTransform.anchoredPosition = pos + toApply;
                    yield return null;
                }

                rectTransform.anchoredPosition = pos;
                yield return null;
            }
        }
    }
}