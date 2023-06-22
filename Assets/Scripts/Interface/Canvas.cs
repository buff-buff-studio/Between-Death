using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

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
            
            var dt = Time.deltaTime * 32f;
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

                        selectionDisplay.position = Vector3.Lerp(selectionDisplay.position, rt.position, dt);
                        selectionDisplay.sizeDelta = Vector2.Lerp(selectionDisplay.sizeDelta, rt.sizeDelta, dt);
                        
                        selectionDisplay.gameObject.SetActive(true);
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