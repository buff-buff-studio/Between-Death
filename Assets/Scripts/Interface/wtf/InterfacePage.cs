using System.Collections;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Refactor.Interface
{
    [RequireComponent(typeof(RectTransform))]
    public class InterfacePage : MonoBehaviour
    {
        public bool sortHorizontally = false;
        private AnimatedWidget[] _animatedWidgets;
        
        public void OnEnable()
        {
            if (sortHorizontally)
                _animatedWidgets = transform.GetComponentsInChildren<AnimatedWidget>().OrderBy((a) => a.rectTransform.anchoredPosition.x).ToArray();
            else
                _animatedWidgets = transform.GetComponentsInChildren<AnimatedWidget>().OrderBy((a) => a.rectTransform.anchoredPosition.y).ToArray();

            Open();
        }

        public void Open()
        {
            StartCoroutine(_HandleAnimWidgets(true));
        }

        public void Close()
        {
            StartCoroutine(_HandleAnimWidgets(false));
        }

        private IEnumerator _HandleAnimWidgets(bool state)
        {
            foreach (var widget in _animatedWidgets)
            {
                widget.state = state;
                yield return new WaitForSeconds(0.1F);
            }
        }
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(InterfacePage))]
    public class InterfacePageEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var page = (target as InterfacePage)!;
            if (GUILayout.Button("Open"))
            {
                page.Open();
            }
            
            if (GUILayout.Button("Close"))
            {
                page.Close();
            }
        }
    }
    #endif
}