using System;
using System.Collections;
using DG.Tweening;
using Refactor.Entities;
using Refactor.Entities.Modules;
using Refactor.Interface.Windows;
using Refactor.Data;
using Refactor.Misc;
using UnityEngine;
using UnityEngine.UI;

namespace Refactor.Interface
{
    public class InGameCanvas : Canvas
    {
        public IngameGameInput ingameGameInput;

        public override void CallAction(InterfaceAction action)
        {
            if (action is InterfaceAction.Start && ingameGameInput.canInput)
            {
                ingameGameInput.canInput = false;
                return;
            }
            
            base.CallAction(action);
        }
    }
}