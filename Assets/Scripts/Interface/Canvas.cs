using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Refactor.Interface
{
    public class InterfaceActionState
    {
        public float NextTime = 0;
        public int Streak = 0;
    }
    
    public class Canvas : MonoBehaviour
    {
        [Header("REFERENCES")] 
        public RectTransform selectionDisplay;
        public BindingDisplayGroup bindingDisplayGroup;

        [Header("SETTINGS")] 
        public float delaySecondInput = 1f;
        public float delayNextInputs = 0.25f;
        
        [FormerlySerializedAs("currentWindows")]
        [Header("STATE")]
        [SerializeField]
        private List<Window> openWindows = new();
        [SerializeField]
        private Window currentWindow;
        [SerializeField]
        private Widget currentWidget;
        private readonly Dictionary<InterfaceAction, InterfaceActionState> _actionStates = new();

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
                selectionDisplay.position = Vector3.zero;
                selectionDisplay.sizeDelta = Vector2.zero;
                
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
                        //selectionDisplay.position = rt.position;
                        //selectionDisplay.sizeDelta = rt.sizeDelta;

                        selectionDisplay.position = Vector3.Lerp(selectionDisplay.position, rt.position, dt);
                        selectionDisplay.sizeDelta = Vector2.Lerp(selectionDisplay.sizeDelta, rt.sizeDelta, dt);
                        
                        selectionDisplay.gameObject.SetActive(true);
                    }
                }
            }
            
            //Actions
            var inputOk = Input.GetKey(KeyCode.Return);
            var inputCancel = Input.GetKey(KeyCode.Escape);
            
            var inputX = Input.GetAxisRaw("Horizontal");
            var inputY = Input.GetAxisRaw("Vertical");
            var inputTriggerLeft = Input.GetKey(KeyCode.Q);
            var inputTriggerRight = Input.GetKey(KeyCode.E);
            var now = Time.time;
            
            HandleInput(inputY > 0.15f, InterfaceAction.MoveUp, now);
            HandleInput(inputY < -0.15f, InterfaceAction.MoveDown, now);
            HandleInput(inputX > 0.15f, InterfaceAction.MoveRight, now);
            HandleInput(inputX < -0.15f, InterfaceAction.MoveLeft, now);
            
            HandleInput(inputOk, InterfaceAction.Confirm, now);
            HandleInput(inputCancel, InterfaceAction.Cancel, now);
                
            HandleInput(inputTriggerLeft, InterfaceAction.TriggerLeft, now);
            HandleInput(inputTriggerRight, InterfaceAction.TriggerRight, now);
        }

        private void _CallAction(InterfaceAction action)
        {
            var nnw = currentWindow != null;
            if (nnw && !currentWindow.readyForInput) return;
            
            if (currentWidget != null && currentWidget.DoAction(action))
                return;

            if (nnw)
                currentWindow.DoAction(action);
        }

        private void HandleInput(bool value, InterfaceAction action, float now)
        {
            var state = _actionStates.GetValueOrDefault(action);
            if (state == null)
            {
                state = new InterfaceActionState() { NextTime = now };
                _actionStates[action] = state;
            }
            
            //Handle it
            if (value)
            {
                if(now > state.NextTime)
                {
                    state.NextTime = now + (state.Streak > 0 ? delayNextInputs : delaySecondInput);
                    state.Streak++;
                    _CallAction(action);
                }
            }
            else
            {
                state.NextTime = now;
                state.Streak = 0;
            }
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