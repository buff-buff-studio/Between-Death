using System;
using System.Collections.Generic;
using UnityEngine;

namespace Refactor.Interface.Windows
{
    [RequireComponent(typeof(CanvasGroup))]
    public class WindowManager : MonoBehaviour
    {
        [Header("WINDOW")] 
        [SerializeField] private string tag = "window";
        [SerializeField] protected List<WindowManager> windows = new List<WindowManager>();
        [SerializeField] protected uint currentWindow = 0;
        [SerializeField] protected uint startWindow = 0;
        private CanvasGroup canvasGroup => GetComponent<CanvasGroup>();
        public string Tag => tag;
        [HideInInspector]
        public bool _active;

        protected virtual void OnEnable()
        {
            AllWindow(false);
            SetWindow(startWindow);
        }

        public void Next()
        {
            windows[(int)currentWindow].SetWindow(false);
            currentWindow++;
            if (currentWindow >= windows.Count) currentWindow = 0;
            SetWindow(currentWindow);
        }
        
        public void Previous()
        {
            windows[(int)currentWindow].SetWindow(false);
            if (currentWindow == 0) currentWindow = (uint) (windows.Count - 1);
            else currentWindow--;
            
            SetWindow(currentWindow);
        }
        
        public void AllWindow(bool active)
        {
            foreach (var window in windows)
            {
                window.SetWindow(active);
            }
        }

        public void SetWindow(string tag)
        {
            SetWindow((uint)windows.FindIndex(x => x.tag == tag));
        }

        public void SetWindow(bool active)
        {
            _active = active;
            canvasGroup.alpha = active ? 1 : 0;
            canvasGroup.interactable = active;
            canvasGroup.blocksRaycasts = active;
        }

        public void SetWindow(uint index)
        {
            if (index >= windows.Count) return;

            windows[(int)currentWindow].SetWindow(false);
            currentWindow = index;
            windows[(int)currentWindow].SetWindow(true);
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (windows.Count == 0) return;
            if (startWindow >= windows.Count) startWindow = 0;
            if (currentWindow >= windows.Count) currentWindow = startWindow;  
            AllWindow(false);
            SetWindow(currentWindow);
        }

#endif
    }
}