using System.Collections;
using Refactor.Audio;
using UnityEngine;

namespace Refactor.Interface.Windows
{
    public class CreditsWindow : Window
    {
        public override void Open()
        {
            base.Open();

            var i = 0;
            foreach (var rt in animatedTransforms)
            {
                StartCoroutine(_DoWoosh(i ++));
            }
        }

        private IEnumerator _DoWoosh(int i)
        {
            yield return new WaitForSeconds(0.25f * i);
            AudioSystem.PlaySound("ui_woosh");
        }
    }
}