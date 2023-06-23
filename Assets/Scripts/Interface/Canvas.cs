using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Refactor.Interface.Widgets;
using Refactor.Interface.Windows;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Refactor.Interface
{
    public class Canvas : MonoBehaviour
    {
        [Header("REFERENCES")] 
        public RectTransform selectionDisplay;
        public BindingDisplayGroup bindingDisplayGroup;
        
        [FormerlySerializedAs("currentWindows")]
        [Header("STATE")]
        [SerializeField]
        private List<Window> openWindows = new();
        [SerializeField]
        private Window currentWindow;
        [SerializeField]
        private Widget currentWidget;
        [SerializeField]
        private ScrollRect currentWidgetScrollRect;

        public void AddActiveWindow(Window window)
        {
            if (!openWindows.Contains(window))
                openWindows.Add(window);

            currentWindow = openWindows.LastOrDefault();
            
            SetCurrentWidget(currentWindow == null ? null : currentWindow.GetFirstWidget(), true);
        }
        
        public void RemoveActiveWindow(Window window)
        {
            if (openWindows.Contains(window))
                openWindows.Remove(window);
            
            currentWindow = openWindows.LastOrDefault();
            SetCurrentWidget(currentWindow == null ? null : currentWindow.GetFirstWidget(), true);
        }

        public Widget GetCurrentWidget()
        {
            return currentWidget;
        }
        
        public void SetCurrentWidget(Widget widget, bool ignore = false)
        {
            if (widget == null)
            {
                if (currentWidget == null && !ignore) return;
                currentWidget = widget;
                UpdateBindingActions();
            }
            else
            {
                currentWidget = widget;
                currentWidgetScrollRect = currentWidget.GetComponentInParent<ScrollRect>();
                UpdateBindingActions();
            }
        }

        public void CloseThenOpen(Window close, Window open)
        {
            StartCoroutine(_CloseThenOpen(close, open));
        }
        
        private IEnumerator _CloseThenOpen(Window close, Window open)
        {
            close.Close();
            yield return new WaitForSeconds(0.5f);
            open.Open();
        }

        public void Update()
        {
            if(currentWindow != null)
                bindingDisplayGroup.gameObject.SetActive(currentWindow.readyForInput);
            else
                bindingDisplayGroup.gameObject.SetActive(true);
            
            var deltaTime = Time.deltaTime;
            if (currentWidget == null)
            {
                selectionDisplay.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
                selectionDisplay.sizeDelta = Vector2.zero; //new Vector2(Screen.width, Screen.height);
                
                if (currentWindow != null)
                    SetCurrentWidget(currentWindow.GetFirstWidget());

                selectionDisplay.gameObject.SetActive(false);
            }
            else
            {
                if(currentWindow != null && !currentWindow.readyForInput)
                    selectionDisplay.gameObject.SetActive(false);
                else
                {
                    if (!currentWidget.gameObject.activeInHierarchy)
                    {
                        SetCurrentWidget(null);
                    }
                    else
                    {
                        var rt = currentWidget.rectTransform;

                        if (selectionDisplay.sizeDelta.x == 0)
                        {
                            selectionDisplay.position = selectionDisplay.position;
                            selectionDisplay.sizeDelta = selectionDisplay.sizeDelta;
                        }

                        selectionDisplay.position = Vector3.Lerp(selectionDisplay.position, rt.position, deltaTime * 32);
                        selectionDisplay.sizeDelta = Vector2.Lerp(selectionDisplay.sizeDelta, rt.sizeDelta, deltaTime * 32);
                        
                        selectionDisplay.gameObject.SetActive(true);
                        
                        //Scroll
                        if (currentWidgetScrollRect != null)
                        {
                            var offset = new Vector3(0, 200, 0);
                            var target = currentWidget.rectTransform;
                            var newAnchor = (Vector2)currentWidgetScrollRect.transform.InverseTransformPoint(currentWidgetScrollRect.content.position)
                                            - (Vector2)currentWidgetScrollRect.transform.InverseTransformPoint(target.position + offset);
                            currentWidgetScrollRect.content.anchoredPosition = Vector2.Lerp(
                                currentWidgetScrollRect.content.anchoredPosition, newAnchor, deltaTime * 32f);

                        }
                    }
                }
            }
        }

        public void CallAction(InterfaceAction action)
        {
            var nnw = currentWindow != null;
            if (nnw && !currentWindow.readyForInput) return;
            
            if (currentWidget != null && currentWidget.DoAction(action))
                return;

            if (nnw)
                currentWindow.DoAction(action);
        }
        
        public void UpdateBindingActions()
        {
            List<string> actions = new();
            
            if(currentWindow != null)
                actions.AddRange(currentWindow.GetBindingActions());
            
            if(currentWidget != null)
                actions.AddRange(currentWidget.GetBindingActions());
            
            if(bindingDisplayGroup != null)
                bindingDisplayGroup.SetVisible(actions);
        }
    }
}