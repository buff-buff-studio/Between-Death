using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Refactor.Interface
{
    public class CanvasInput : MonoBehaviour
    {
        public class InterfaceActionState
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
        public InputAction inputMove;
        public InputAction inputConfirm;
        public InputAction inputCancel;
        
        public void OnEnable()
        {
            inputMove.Enable();
            inputConfirm.Enable();
            inputCancel.Enable();
        }

        private void OnDisable()
        {
            inputMove.Disable();
            inputConfirm.Disable();
            inputCancel.Disable();
        }

        private void Update()
        {
            var inputXY = inputMove.ReadValue<Vector2>();

            var inputYES = inputConfirm.ReadValue<float>() > 0;
            var inputNO = inputCancel.ReadValue<float>() > 0;
            
            var inputTriggerLeft = Input.GetKey(KeyCode.Q);
            var inputTriggerRight = Input.GetKey(KeyCode.E);
            var now = Time.time;
            
            HandleInput(inputXY.y > 0.15f, InterfaceAction.MoveUp, now);
            HandleInput(inputXY.y < -0.15f, InterfaceAction.MoveDown, now);
            HandleInput(inputXY.x > 0.15f, InterfaceAction.MoveRight, now);
            HandleInput(inputXY.x < -0.15f, InterfaceAction.MoveLeft, now);
            
            HandleInput(inputYES, InterfaceAction.Confirm, now);
            HandleInput(inputNO, InterfaceAction.Cancel, now);
                
            HandleInput(inputTriggerLeft, InterfaceAction.TriggerLeft, now);
            HandleInput(inputTriggerRight, InterfaceAction.TriggerRight, now);
        }
        
        
        public void HandleInput(bool value, InterfaceAction action, float now)
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
                    canvas.CallAction(action);
                }
            }
            else
            {
                state.NextTime = now;
                state.Streak = 0;
            }
        }
    }
}