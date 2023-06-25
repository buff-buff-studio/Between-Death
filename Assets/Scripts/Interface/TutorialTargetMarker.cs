using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Refactor.Interface
{
    public class TutorialTargetMarker : MonoBehaviour
    {
        public CanvasGroup canvasGroup;
        public float radius = 200;
        public Image image;
        public TMP_Text distanceLabel;
        public Color color = Color.red;
        public Vector3 targetPos = new(0, 0, 0);
        private Camera _camera;
        public CanvasGroup innerCanvasGroup;
        public float threshold = 5f;

        private void OnEnable()
        {
            _camera = Camera.main;
        }
        
        private void Update()
        {
            var delta = (targetPos - _camera.transform.position);
            var distance = math.max(0, delta.magnitude - threshold);

            var vw = _camera.ScreenToWorldPoint(new Vector2(Screen.width / 2f, Screen.height / 2f));
            var dir = (targetPos - vw).normalized;
            var lc = _camera.transform.InverseTransformDirection(dir);
            lc = new Vector3(lc.x, lc.y, 0).normalized;
 
            image.transform.localPosition = lc * radius;
            image.transform.eulerAngles = new Vector3(0, 0, Angle(lc));
            distanceLabel.transform.eulerAngles = Vector3.zero;
            innerCanvasGroup.alpha = Mathf.Clamp01(Mathf.Abs(lc.x));

            image.color = color;
            distanceLabel.color = color;
            distanceLabel.text = $"{distance:F0}m";
        }
        
        public static float Angle(Vector2 v)
        {
            return Vector2.SignedAngle(new Vector2(0, -1), v);
        }
    }
}