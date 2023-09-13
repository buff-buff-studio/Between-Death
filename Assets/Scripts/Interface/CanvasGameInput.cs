using System.Collections.Generic;
using Refactor.Interface.Windows;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Refactor.Interface
{
    public enum InterfaceAction
    {
        Start,
        
        Confirm,
        Cancel,
        
        MoveLeft,
        MoveRight,
        MoveUp,
        MoveDown,
        
        TriggerLeft, 
        TriggerRight,
        
        ActionThird,
        ActionForth,
    }
    
    public class CanvasGameInput : GameInput
    {
        private class InterfaceActionState
        {
            public float NextTime = 0;
            public int Streak = 0;
        }
        
        [Header("SETTINGS")] 
        public float delaySecondInput = 0.5f;

        public float delayNextInputs = 0.125f;
        public Canvas canvas;
        private readonly Dictionary<InterfaceAction, InterfaceActionState> _actionStates = new();

        [Header("INPUT")] 
        public InputAction inputStart;
        public InputAction inputMove;
        public InputAction inputConfirm;
        public InputAction inputCancel;
        public InputAction inputTrigger;
        public InputAction inputThird;
        public InputAction inputForth;
        
        protected override void OnEnable()
        {
            inputMove.Enable();
            inputConfirm.Enable();
            inputCancel.Enable();
            inputTrigger.Enable();
            inputThird.Enable();
            inputForth.Enable();
            inputStart.Enable();
            
            base.OnEnable();
        }
        

        protected override void OnDisable()
        {
            inputMove.Disable();
            inputConfirm.Disable();
            inputCancel.Disable();
            inputTrigger.Disable();
            inputThird.Disable();
            inputForth.Disable();
            inputStart.Disable();
            
            base.OnDisable();
        }
        
        protected override void Update()
        {
            base.Update();

            if (canvas == null)
                return;
            
            if(CurrentControlScheme == ControlScheme.Desktop)
                if(canvas.currentWindow == null || canvas.currentWindow is not SaveWindow)
                    canvas.SetCurrentWidget(null);
            
            var readInputXY = inputMove.ReadValue<Vector2>();
            var readInputStart = inputStart.ReadValue<float>() > 0;
            var readInputYes = inputConfirm.ReadValue<float>() > 0;
            var readInputNo = inputCancel.ReadValue<float>() > 0;
            var readInputTrigger = inputTrigger.ReadValue<float>();
            
            var readInputThird = inputThird.ReadValue<float>() > 0;
            var readInputForth = inputForth.ReadValue<float>() > 0;
            
            var now = Time.time;

            _HandleInputXY(readInputXY, InterfaceAction.MoveRight, InterfaceAction.MoveLeft, InterfaceAction.MoveUp,
                InterfaceAction.MoveDown, now);
 
            _HandleInput(readInputYes, InterfaceAction.Confirm, now);
            _HandleInput(readInputNo, InterfaceAction.Cancel, now);
            
            _HandleInput(readInputTrigger < -0.15f, InterfaceAction.TriggerLeft, now);
            _HandleInput(readInputTrigger > 0.15f, InterfaceAction.TriggerRight, now);
            
            _HandleInput(readInputThird, InterfaceAction.ActionThird, now);
            _HandleInput(readInputForth, InterfaceAction.ActionForth, now);
            _HandleInput(readInputStart, InterfaceAction.Start, now);
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