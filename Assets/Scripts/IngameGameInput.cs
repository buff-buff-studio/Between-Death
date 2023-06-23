using UnityEngine;
using UnityEngine.InputSystem;

namespace Refactor
{
    public class IngameGameInput : GameInput
    {
        public class InputBool
        {
            public bool value {get; private set;}
            public bool trigger {get; private set;}

            public void SetValue(bool v)
            {
                this.trigger = v && !value;
                this.value = v;
            }
        }
        
        public static readonly InputBool InputInteract = new();
        public static Vector2 InputMove = Vector2.zero;
        public static Vector2 InputCamera = Vector2.zero;
        public static readonly InputBool InputJump = new();
        public static readonly InputBool InputChangeElement = new();
        public static readonly InputBool InputAttack0 = new();
        public static readonly InputBool InputAttack1 = new();
        public static readonly InputBool InputDash = new();
        public static readonly InputBool InputRunning = new();
        
        public InputAction inputInteract;
        public InputAction inputMove;
        public InputAction inputCamera;
        public InputAction inputJump;
        public InputAction inputChangeElement;
        public InputAction inputAttack0;
        public InputAction inputAttack1;
        public InputAction inputDash;
        public InputAction inputRunning;

        protected override void OnEnable()
        {
            base.OnEnable();
            inputInteract.Enable();
            inputCamera.Enable();
            inputMove.Enable();
            inputJump.Enable();
            inputAttack0.Enable();
            inputAttack1.Enable();
            inputChangeElement.Enable();
            inputDash.Enable();
            inputRunning.Enable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            inputInteract.Disable();
            inputCamera.Disable();
            inputMove.Disable();
            inputJump.Disable();
            inputAttack0.Disable();
            inputAttack1.Disable();
            inputChangeElement.Disable();
            inputDash.Disable();
            inputRunning.Disable();
        }

        protected override void Update()
        {
            base.Update();
            
            InputInteract.SetValue(inputInteract.ReadValue<float>() > 0);
            InputMove = inputMove.ReadValue<Vector2>();
            InputCamera = inputCamera.ReadValue<Vector2>();
            InputJump.SetValue(inputJump.ReadValue<float>() > 0);
            
            InputAttack0.SetValue(inputAttack0.ReadValue<float>() > 0);
            InputAttack1.SetValue(inputAttack1.ReadValue<float>() > 0);
            
            InputChangeElement.SetValue(inputChangeElement.ReadValue<float>() > 0);
            InputDash.SetValue(inputDash.ReadValue<float>() > 0);
            InputRunning.SetValue(inputRunning.ReadValue<float>() > 0);
        }
    }
}