using System;

namespace Refactor.Tutorial.Steps
{
    public class InventoryTutorialStep : DefaultTutorialStep
    {
        public InGameMenu menu;
        
        public override void OnBegin()
        {
            base.OnBegin();
            input.DisableAllInput();
            input.canMenu = true;
            menu.onChangeMenuOpen.AddListener(OnChangeMenuOpen);
            
            controller.ShowBindingDisplay("inventory");
        }

        private void OnChangeMenuOpen(bool v)
        {
            if(!v)
                controller.NextStep();
        }
        

        public override void OnEnd()
        {
            base.OnEnd();
            input.DisableAllInput();
            
            menu.onChangeMenuOpen.RemoveListener(OnChangeMenuOpen);
            controller.ShowBindingDisplay("");
        }
    }
}