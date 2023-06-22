using UnityEngine;

namespace Refactor.Interface
{
    public class InterfaceController : MonoBehaviour
    {
        public float timeToMove = 0f;
        public RectTransform navigationHighlighting;
        public Widget currentWidget;
        
        private void OnEnable()
        {
            #region Navigation Frame
            navigationHighlighting.position = currentWidget.rectTransform.position;
            navigationHighlighting.sizeDelta = currentWidget.rectTransform.sizeDelta;
            #endregion
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            
            #region Navigation Frame
            navigationHighlighting.position = Vector3.Lerp(
                navigationHighlighting.position, currentWidget.rectTransform.position, deltaTime * 12f);
            navigationHighlighting.sizeDelta = Vector2.Lerp(
                navigationHighlighting.sizeDelta, currentWidget.rectTransform.sizeDelta, deltaTime * 12f);
            #endregion
            
            #region Navigation Input
            var dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            
            if (dir.magnitude > 0.1f && Time.time > timeToMove)
            {
                timeToMove = Time.time + 0.2f;

                var nextWidget = currentWidget.GetNextNavigable(dir);
                
                if(nextWidget != null)
                    currentWidget = nextWidget;
            }
            #endregion
        }
    }
}