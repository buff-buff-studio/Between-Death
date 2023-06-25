using Refactor.Interface;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Refactor
{
    public class IngameGameInput : CanvasGameInput
    {
        public class InputBool
        {
            public bool value {get; private set;}
            public bool trigger {get; private set;}

            public void SetValue(bool v)
            {
                trigger = v && !value;
                value = v;
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
        
        [Header("SETTINGS")]
        public bool canInput = true;

        public bool canMove = true;
        public bool canInteract = true;
        public bool canMoveCamera = true;
        public bool canJump = true;
        public bool canChangeElement = true;
        public bool canAttack0 = true;
        public bool canAttack1 = true;
        public bool canDash = true;
        public bool canRun = true;
        
        [Header("GAME INPUT")]
        public InputAction inputInteract;
        public InputAction inputCamera;
        public InputAction inputJump;
        public InputAction inputChangeElement;
        public InputAction inputAttack0;
        public InputAction inputAttack1;
        public InputAction inputDash;
        public InputAction inputRunning;

        public void DisableAllInput()
        { 
            canMove = false;
            canInteract = false;
            //canMoveCamera = false;
            canJump = false;
            canChangeElement = false;
            canAttack0 = false;
            canAttack1 = false;
            canDash = false;
            canRun = false;
        }
        
        public void EnableAllInput()
        { 
            canMove = true;
            canInteract = true;
            canMoveCamera = true;
            canJump = true;
            canChangeElement = true;
            canAttack0 = true;
            canAttack1 = true;
            canDash = true;
            canRun = true;
        }

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
            
            Cursor.lockState = canInput ? CursorLockMode.Locked : CursorLockMode.None;
            
            InputInteract.SetValue(canInput && canInteract && inputInteract.ReadValue<float>() > 0);
            InputMove = canInput && canMove ? inputMove.ReadValue<Vector2>() : Vector2.zero;
            InputCamera = canInput && canMoveCamera ? -inputCamera.ReadValue<Vector2>() : Vector2.zero;
            InputJump.SetValue(canInput && canJump && inputJump.ReadValue<float>() > 0);
            
            InputAttack0.SetValue(canInput && canAttack0 && inputAttack0.ReadValue<float>() > 0);
            InputAttack1.SetValue(canInput && canAttack1 && inputAttack1.ReadValue<float>() > 0);
            
            InputChangeElement.SetValue(canInput && canChangeElement && inputChangeElement.ReadValue<float>() > 0);
            InputDash.SetValue(canInput && canDash && inputDash.ReadValue<float>() > 0);
            InputRunning.SetValue(canInput && canRun && inputRunning.ReadValue<float>() > 0);
        }
    }
}