using System;
using Refactor.Data.Variables;
using Refactor.Entities;
using Refactor.Entities.Modules;
using Refactor.Interface;
using Refactor.Misc;
using Refactor.Props;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

namespace Refactor
{
    public class GameController : Singleton<GameController>
    {
        [Header("REFERENCES")]
        public Entity player;
        public OrbitCamera camera;
        public Variable[] toReset;
        
        private InteractionEntityModule _interactionPlayerModule;
        private void Awake()
        {
            foreach(var v in toReset)
                v.Reset();
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
        }
    }
}