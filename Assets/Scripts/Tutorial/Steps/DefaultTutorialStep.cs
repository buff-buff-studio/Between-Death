using System;
using UnityEngine;

namespace Refactor.Tutorial.Steps
{
    public class DefaultTutorialStep : TutorialStep
    {
        public string title;
        public string content;

        public override void OnBegin()
        {
            base.OnBegin();
            controller.ShowTutorialBox(title, content);
        }
    }
}