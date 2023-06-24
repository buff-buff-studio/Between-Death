using System.Collections;
using DG.Tweening;
using Refactor.Interface.Windows;
using UnityEngine;

namespace Refactor.Interface
{
    public class MenuOnOpen : MonoBehaviour
    {
        public MainMenuWindow window;
        public CanvasGroup canvasGroup;

        public void Start()
        {
            StartCoroutine(_OpenAnim());
        }

        private IEnumerator _OpenAnim()
        {
            window.Open();
            yield return new WaitForSeconds(1f);
            canvasGroup.DOFade(0f, 3f);
            yield return new WaitForSeconds(3f);
            Destroy(this.gameObject);
        }
    }
}