using System.Collections.Generic;
using DG.Tweening;
using Refactor.Audio;
using Refactor.Interface.Widgets;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Refactor.Interface.Windows
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Window : Widget
    {
        [Header("WINDOW")]
        public string id;

        public bool readyForInput = false;
        public RectTransform[] animatedTransforms;

        public Window openOnClose;
        public bool canClose;

        public bool isOpen => gameObject.activeSelf;

        protected override void OnEnable()
        {
            base.OnEnable();
            readyForInput = true;
            if (canvas == null) return;
            canvas.AddActiveWindow(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (canvas == null) return;
            canvas.RemoveActiveWindow(this);
        }

        public virtual void Open()
        {
            gameObject.SetActive(true);
            readyForInput = false;
            canvas.AddActiveWindow(this);
            
            var tweenId = $"window_{id}";
            DOTween.Kill(tweenId);
            DOTween.Kill($"{tweenId}_child");
            var group = gameObject.GetOrAddComponent<CanvasGroup>();
            group.alpha = 0;
            group.DOFade(1, 0.5f).SetId(tweenId);
            group.interactable = false;
            var hasAnim = animatedTransforms.Length;
            
            var rectTransform = group.GetRectTransform();
            var p = rectTransform.localPosition;
            rectTransform.DOPunchPosition(new Vector3(0, 20, 0), 0.75f, 8, 1f).SetId(tweenId).OnComplete(() =>
            {
                rectTransform.localPosition = p;
                group.interactable = true;
                
                if(hasAnim == 0) readyForInput = true;
            }).OnKill(() =>
            {
                rectTransform.localPosition = p;
            });

            var delay = 0.05f;
            var offset = new Vector3(100, 0, 0);
            foreach (var rt in animatedTransforms)
            {
                var cg = rt.GetOrAddComponent<CanvasGroup>(out var wasAdded);
                cg.alpha = 0;
                var pos = rt.localPosition;
                rt.localPosition += offset;

                var tween = cg.DOFade(1f, 1f).SetDelay(delay).SetId($"{tweenId}_child");
                if(wasAdded)
                    tween.OnKill(() => Destroy(cg)).OnComplete(() => Destroy(cg));
                rt.DOLocalMove(pos, 1f, true).SetDelay(delay).SetId($"{tweenId}_child").OnKill(() =>
                    {
                        rt.localPosition = pos;
                    }).OnComplete(() =>
                    {
                        rt.localPosition = pos;
                        
                        if (--hasAnim == 0) 
                            readyForInput = true;
                    });
                delay += 0.25f;
            }
        }

        public virtual void Close()
        {
            AudioSystem.PlaySound("ui_window");
            
            var tweenId = $"window_{id}";
            DOTween.Kill(tweenId);
            var group = gameObject.GetOrAddComponent<CanvasGroup>();
            var rectTransform = group.GetRectTransform();
            var pos = rectTransform.localPosition;
            
            var tween = rectTransform.DOPunchPosition(new Vector3(0, -50, 0), 2f, 0, 1f).SetId(tweenId);
            
            group.interactable = false;
            readyForInput = false;
            group.DOFade(0, 0.5f).SetId(tweenId).OnComplete(() =>
            {
                gameObject.SetActive(false);
                rectTransform.localPosition = pos;
                tween.Kill();
                DOTween.Kill($"{tweenId}_child");
            }).OnKill(() =>
            {
                rectTransform.localPosition = pos;
            });
        }

        public virtual Widget GetFirstWidget() => null;

        public override bool DoAction(InterfaceAction action)
        {
            if (action == InterfaceAction.Cancel)
            {
                if (canClose)
                {
                    if(openOnClose)
                        canvas.CloseThenOpen(this, openOnClose);
                    else
                        Close();
                }
            }
            
            return base.DoAction(action);
        }
        
        public override IEnumerable<string> GetBindingActions()
        {
            if (canClose)
                yield return openOnClose == null ? "close" : "back";
        }
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(Window), true)]
    public class WindowEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var window = (target as Window)!;
            if (Application.isPlaying)
            {
                if (GUILayout.Button("Open"))
                {
                    window.Open();
                }

                if (GUILayout.Button("Close"))
                {
                    window.Close();
                }
            }
        }
    }
    #endif
}