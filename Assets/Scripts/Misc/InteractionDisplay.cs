using System;
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
            GameInput.OnChangeControlScheme += _ReloadSprite;
            _ReloadSprite();
        }

        private void OnDisable()
        {
            GameInput.OnChangeControlScheme -= _ReloadSprite;
        }

        private void _ReloadSprite()
        {
            imageSprite.sprite = palette.GetSprite(GameInput.CurrentControlScheme);
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