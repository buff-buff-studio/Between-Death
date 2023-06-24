using DG.Tweening;
using Refactor;
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
            _SwitchTabAnimation(prevTab, currentTab, -1);
        }

        public void PrevTab()
        {
            if (tabs.Length < 2) return;
            
            var prevTab = currentTab;
            currentTab = currentTab > 0 ? currentTab - 1 : tabs.Length - 1;
            _SwitchTabAnimation(prevTab, currentTab, 1);
        }

        public void SetTab(int tab)
        {
            var prevTab = currentTab;
            currentTab = tab;
            
            _SwitchTabAnimation(prevTab, currentTab, (int) math.sign(currentTab - prevTab));
        }

        private void _SwitchTabAnimation(int prev, int current, int dir)
        {
            if (prev == current) return;
            canvas.SetCurrentWidget(null);
            
            var moveDir = new Vector2(dir * -50, 0);
            DOTween.Kill(GetInstanceID());

            readyForInput = false;
            _DoTabAnimation(prev, false, moveDir);
            _DoTabAnimation(current, true, moveDir);
        }

        private void _DoTabAnimation(int tab, bool open, Vector2 dir)
        {
            var rt = tabs[tab];
            var cg = rt.GetOrAddComponent<CanvasGroup>(out var wasAdded);
            
            var tween = cg.DOFade(open ? 1 : 0, 1f).SetId(GetInstanceID());

            var prevPos = rt.localPosition;
            var to = prevPos;
            cg.interactable = false;
            
            if (open)
            {
                var scrollRect = rt.gameObject.GetComponentInChildren<ScrollRect>();
                if (scrollRect != null)
                    scrollRect.verticalNormalizedPosition = 1;
                rt.SetSiblingIndex(1);
                cg.alpha = 0;
                rt.gameObject.SetActive(true);
                
                rt.localPosition += new Vector3(dir.x, dir.y, 0);
                rt.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                rt.DOScale(new Vector3(1f, 1f, 1f), 0.5f);
            }
            else
            {
                tween.OnComplete(() => rt.gameObject.SetActive(false));
                tween.OnKill(() => rt.gameObject.SetActive(false));
                to -= new Vector3(dir.x, dir.y, 0);
                rt.DOScale(new Vector3(0.75f, 0.75f, 0.75f), 0.5f);
            }
            
            rt.DOLocalMove(to, 1f)
                .OnComplete(() =>
                {
                    if (open)
                    {
                        cg.interactable = true;
                        readyForInput = true;
                    }

                    rt.localPosition = prevPos;
                    rt.localScale = Vector3.one;
                }).OnKill(() =>
                {
                    rt.localPosition = prevPos;
                    rt.localScale = Vector3.one;
                }).SetId(GetInstanceID());
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