using System.Collections;
using Refactor.I18n;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Refactor.Interface
{
    public class LoadingScreen : MonoBehaviour
    {
        public static string Scene = "Scenes/Game_Playground";

        public static void LoadScene(string scene)
        {
            Cursor.lockState = CursorLockMode.None;
            Scene = scene;
            SceneManager.LoadScene("Scenes/Loading");
        }
        
        [Header("REFERENCES")]
        public RectTransform progressBarBackground;
        public RectTransform progressBarFill;
        public TMP_Text labelTip;
        public TMP_Text labelLoading;
        public TMP_Text targetSceneLabel;
        
        [Header("SETTINGS")]
        public int tipCount = 5;
        
        [Header("STATE")]
        public float time = 0;

        public string loadingText;
        [Range(0f, 1f)]
        public float progress = 0;

        private void OnEnable()
        {
            loadingText = LanguageManager.Localize("ui.loading.loading");
            labelTip.text = LanguageManager.Localize($"ui.loading.tips.{Random.Range(0, tipCount)}");
            var slc = Scene.Split("/");
            targetSceneLabel.text = LanguageManager.Localize($"ui.loading.scene.{slc[^1].ToLower()}");
            
            StartCoroutine(_LoadSceneCoroutine());
        }

        private IEnumerator _LoadSceneCoroutine()
        {
            yield return new WaitForSeconds(1f);
            var asyncLoad = SceneManager.LoadSceneAsync(Scene);
            asyncLoad.allowSceneActivation = false;
            
            while (!asyncLoad.isDone)
            {
                progress = asyncLoad.progress;
                if (asyncLoad.progress >= 0.9f)
                {
                    progress = 1f;
                    yield return new WaitForSeconds(1f);
                    asyncLoad.allowSceneActivation = true;
                    Cursor.lockState = CursorLockMode.None;
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            
            time += deltaTime * 3f;
            var i = loadingText.Length - ((int)time % 4);
            labelLoading.text = $"{loadingText[..i]}<color=#00000000>{loadingText[i..]}</color>";
            
            progressBarFill.sizeDelta =
                Vector2.Lerp(progressBarFill.sizeDelta,
                    new Vector2(progressBarBackground.rect.width * progress, progressBarFill.sizeDelta.y), deltaTime * 4f);
        }
    }
}