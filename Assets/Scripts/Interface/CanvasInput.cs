using System;
using System.Collections.Generic;
using Refactor.Interface.Widgets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

namespace Refactor.Interface
{
    public class CanvasInput : MonoBehaviour
    {
        public static ControlScheme controlScheme = ControlScheme.Desktop;
        public static Action<ControlScheme> OnChangeControlScheme;
        
        public class InterfaceActionState
        {
            public float NextTime = 0;
            public int Streak = 0;
        }

        public enum ControlScheme
        {
            Desktop,
            Gamepad,
            Xbox,
            Playstation
        }

        [Header("SETTINGS")] 
        public float delaySecondInput = 0.5f;

        public float delayNextInputs = 0.125f;
        public Canvas canvas;
        private readonly Dictionary<InterfaceAction, InterfaceActionState> _actionStates = new();

        [Header("INPUT")] 
        public InputAction inputMove;
        public InputAction inputConfirm;
        public InputAction inputCancel;
        public InputAction inputTrigger;
        
        public void OnEnable()
        {
            inputMove.Enable();
            inputConfirm.Enable();
            inputCancel.Enable();
            inputTrigger.Enable();

            Invoke(nameof(ReloadScheme), 0.1f);
        }

        public void ReloadScheme()
        {
            OnChangeControlScheme?.Invoke(controlScheme);
        }

        private void OnDisable()
        {
            inputMove.Disable();
            inputConfirm.Disable();
            inputCancel.Disable();
            inputTrigger.Disable();
        }

        private void SetControlScheme(ControlScheme scheme)
        {
            if (scheme == controlScheme) return;

            controlScheme = scheme;
            ReloadScheme();
        }
        
        private void Update()
        {
            var gamepad = Gamepad.current;
            
            switch (gamepad)
            {
                case XInputController:
                    SetControlScheme(ControlScheme.Xbox);
                    break;
                case DualShockGamepad:
                    SetControlScheme(ControlScheme.Playstation);
                    break;
                case not null:
                    SetControlScheme(ControlScheme.Gamepad);
                    break;
                default:
                    SetControlScheme(ControlScheme.Desktop);
                    break;
            }
            
            if(controlScheme == ControlScheme.Desktop)
                canvas.SetCurrentWidget(null);
            
            var readInputXY = inputMove.ReadValue<Vector2>();
            var readInputYes = inputConfirm.ReadValue<float>() > 0;
            var readInputNo = inputCancel.ReadValue<float>() > 0;
            var readInputTrigger = inputTrigger.ReadValue<float>();
            
            var now = Time.time;

            _HandleInputXY(readInputXY, InterfaceAction.MoveRight, InterfaceAction.MoveLeft, InterfaceAction.MoveUp,
                InterfaceAction.MoveDown, now);
 
            _HandleInput(readInputYes, InterfaceAction.Confirm, now);
            _HandleInput(readInputNo, InterfaceAction.Cancel, now);
                
            _HandleInput(readInputTrigger < -0.15f, InterfaceAction.TriggerLeft, now);
            _HandleInput(readInputTrigger > 0.15f, InterfaceAction.TriggerRight, now);
        }
        
        
        private void _HandleInput(bool value, InterfaceAction action, float now)
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
                if (!(now > state.NextTime)) return;
                state.NextTime = now + (state.Streak > 0 ? delayNextInputs : delaySecondInput);
                state.Streak++;
                canvas.CallAction(action);
            }
            else
            {
                state.NextTime = now;
                state.Streak = 0;
            }
        }
        
        private void _HandleInputXY(Vector2 value, InterfaceAction a, InterfaceAction b, InterfaceAction c, InterfaceAction d, float now)
        {
            var state = _actionStates.GetValueOrDefault(a);
            if (state == null)
            {
                state = new InterfaceActionState() { NextTime = now };
                _actionStates[a] = state;
            }

            var isA = value.x > 0.8f; //right
            var isB = value.x < -0.8f; //left
            var isC = value.y > 0.8f; //up
            var isD = value.y < -0.8f; //down
            
            //Handle it
            if (isA || isB || isC || isD)
            {
                if (!(now > state.NextTime)) return;
                state.NextTime = now + (state.Streak > 0 ? delayNextInputs : delaySecondInput);
                state.Streak++;
                
                if(isA) canvas.CallAction(a);
                if(isB) canvas.CallAction(b);
                if(isC) canvas.CallAction(c);
                if(isD) canvas.CallAction(d);
            }
            else
            {
                state.NextTime = now;
                state.Streak = 0;
            }
        }
    }
}