using Refactor.Interface;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Refactor.Misc
{
    public class InteractionDisplay : MonoBehaviour
    {
        private Transform _camera;
        public Image interactionProgress;
        public float progress = 0f;
        public Image imageSprite;
        public BindingDisplay.SpritePalette palette;
        
        private void OnEnable()
        {
            _camera = Camera.main!.transform;
            //CanvasInput.controlScheme
        }

        private void _ReloadSprite()
        {
            
        }

        public void Update()
        {
            var t = transform;
            var fw = _camera.transform.position - t.position;
            t.forward = -fw;

            interactionProgress.fillAmount = progress;
        }
    }
}