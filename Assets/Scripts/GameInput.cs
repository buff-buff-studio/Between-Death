using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

namespace Refactor
{
    public class GameInput : Singleton<GameInput>
    {
        public static ControlScheme CurrentControlScheme = ControlScheme.Desktop;
        public static Action OnChangeControlScheme;
        
        public enum ControlScheme
        {
            Desktop,
            Gamepad,
            Xbox,
            Playstation
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            Invoke(nameof(ReloadScheme), 0.1f);
        }
        
        public void ReloadScheme()
        {
            OnChangeControlScheme?.Invoke();
        }
        
        private void SetControlScheme(ControlScheme scheme)
        {
            if (scheme == CurrentControlScheme) return;

            CurrentControlScheme = scheme;
            ReloadScheme();
        }

        protected virtual void Update()
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
        }
    }
}