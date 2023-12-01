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

        public static readonly InputBool InputSkill0 = new();
        public static readonly InputBool InputSkill1 = new();
        public static readonly InputBool InputSkill2 = new();
        
        [Header("SETTINGS")]
        public static bool CanInput = true;

        public bool canInput
        {
            get => CanInput;
            set => CanInput = value;
        }

        [Space]
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
        
        public InputAction inputSkill0;
        public InputAction inputSkill1;
        public InputAction inputSkill2;

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
            inputSkill0.Enable();
            inputSkill1.Enable();
            inputSkill2.Enable();
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
            inputSkill0.Disable();
            inputSkill1.Disable();
            inputSkill2.Enable();
        }

        protected override void Update()
        {
            base.Update();
            
            Cursor.lockState = CanInput ? CursorLockMode.Locked : CursorLockMode.None;
            
            InputInteract.SetValue(CanInput && canInteract && inputInteract.ReadValue<float>() > 0);
            InputMove = CanInput && canMove ? inputMove.ReadValue<Vector2>() : Vector2.zero;
            InputCamera = CanInput && canMoveCamera ? -inputCamera.ReadValue<Vector2>() : Vector2.zero;
            InputJump.SetValue(CanInput && canJump && inputJump.ReadValue<float>() > 0);
            
            InputAttack0.SetValue(CanInput && canAttack0 && inputAttack0.ReadValue<float>() > 0);
            InputAttack1.SetValue(CanInput && canAttack1 && inputAttack1.ReadValue<float>() > 0);
            
            InputChangeElement.SetValue(CanInput && canChangeElement && inputChangeElement.ReadValue<float>() > 0);
            InputDash.SetValue(CanInput && canDash && inputDash.ReadValue<float>() > 0);
            InputRunning.SetValue(CanInput && canRun && inputRunning.ReadValue<float>() == 0);
            
            InputSkill0.SetValue(CanInput && inputSkill0.ReadValue<float>() > 0);
            InputSkill1.SetValue(CanInput && inputSkill1.ReadValue<float>() > 0);
            InputSkill2.SetValue(CanInput && inputSkill2.ReadValue<float>() > 0);
        }
    }
}