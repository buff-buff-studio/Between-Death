using Refactor.Entities;

namespace Refactor.Tutorial.Steps
{
    public class ChangeElementTutorialStep : DefaultTutorialStep
    {
        public Entity player;
        public override void OnBegin()
        {
            base.OnBegin();
            
            input.DisableAllInput();
            input.canMoveCamera = true;
            input.canChangeElement = true;
            
            controller.ShowBindingDisplay("change_element");

            player.onChangeElement.AddListener(controller.NextStep);
        }
        
        public override void OnEnd()
        {
            base.OnEnd();
            player.onChangeElement.RemoveListener(controller.NextStep);
            controller.ShowBindingDisplay("");
            input.DisableAllInput();
        }
    }
}