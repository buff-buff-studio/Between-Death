using DG.Tweening;
using Refactor;
using Refactor.Audio;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Refactor.Interface.Widgets
{
    public class TabWidget : Widget
    {
        public RectTransform[] tabs;
        public ButtonWidget[] tabHeaders;
        public Widget[] tabFirstWidgets;
        public int currentTab = 0;
        public bool readyForInput = true;

        protected override void OnEnable()
        {
            base.OnEnable();
            currentTab %= tabs.Length;
            for(var i = 0; i < tabs.Length; i ++)
                tabs[i].gameObject.SetActive(i == currentTab);

            for (var i = 0; i < tabHeaders.Length; i++)
            {
                var n = i;
                var headers = tabHeaders[i];
                headers.onPress.RemoveAllListeners();
                headers.onPress.AddListener(() => SetTab(n));
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            DOTween.Kill(GetInstanceID());
        }

        protected override void Update()
        {
            base.Update();
            
            var dt = Time.deltaTime * 16f;
            for(var i = 0; i < tabHeaders.Length; i ++)
                tabHeaders[i].rectTransform.localScale = Vector3.Lerp(tabHeaders[i].rectTransform.localScale, i == currentTab ? Vector3.one : new Vector3(0.8f, 0.8f, 0.8f), dt);
        }

        public void NextTab()
        {
            if (tabs.Length < 2) return;
            
            var prevTab = currentTab;
            currentTab = (currentTab + 1) % tabs.Length;
            tabs[prevTab].gameObject.SetActive(false);
            tabs[currentTab].gameObject.SetActive(true);
            AudioSystem.PlaySound("ui_window");
        }

        public void PrevTab()
        {
            if (tabs.Length < 2) return;
            
            var prevTab = currentTab;
            currentTab = currentTab > 0 ? currentTab - 1 : tabs.Length - 1;

            tabs[prevTab].gameObject.SetActive(false);
            tabs[currentTab].gameObject.SetActive(true);
            AudioSystem.PlaySound("ui_window");
        }

        public void SetTab(int tab)
        {
            var prevTab = currentTab;
            currentTab = tab;
            
            tabs[prevTab].gameObject.SetActive(false);
            tabs[currentTab].gameObject.SetActive(true);
        }
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(TabWidget))]
    public class TabWidgetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var tab = (target as TabWidget)!;
            if (Application.isPlaying)
            {
                if (GUILayout.Button("Prev Tag"))
                {
                    tab.PrevTab();
                }

                if (GUILayout.Button("Next Tab"))
                {
                    tab.NextTab();
                }
            }
        }
    }
    #endif
}